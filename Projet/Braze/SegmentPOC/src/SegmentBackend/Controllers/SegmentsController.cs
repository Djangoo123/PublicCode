
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using SegmentBackend.Data;
using SegmentBackend.Models;
using SegmentBackend.Services;

namespace SegmentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class SegmentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public SegmentsController(AppDbContext db, JsonSerializerOptions jsonOptions) => _db = db;

        [HttpPost("preview")]
        public async Task<ActionResult<object>> Preview([FromBody] JsonElement segmentJson, [FromQuery] int take = 25, [FromQuery] int timeoutMs = 5000, CancellationToken ct = default)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(Math.Clamp(timeoutMs, 500, 15000));

            var def = JsonSerializer.Deserialize<SegmentDefinition>(segmentJson.GetRawText(), _json)!;
            var predicate = SegmentToExpression.BuildPredicate(def.Root);

            var query = _db.Users.AsNoTracking().Where(predicate);

            var countTask = query.CountAsync(cts.Token);
            var sampleTask = query.Take(Math.Clamp(take, 1, 200))
                                  .Select(u => new { u.Id, u.Country, u.Plan, u.OrdersCount, u.TotalSpend, u.CreatedUtc })
                                  .ToListAsync(cts.Token);

            await Task.WhenAll(countTask, sampleTask);
            return Ok(new { Count = countTask.Result, Sample = sampleTask.Result });
        }

        [HttpPost("save")]
        public async Task<ActionResult<object>> Save([FromBody] SegmentDefinition def, [FromQuery] string createdBy = "ui")
        {
            var entity = new SegmentEntity
            {
                Name = def.Name,
                Json = JsonSerializer.Serialize(def, _json),
                Version = 1,
                CreatedBy = createdBy
            };
            _db.Segments.Add(entity);
            _db.SegmentRevisions.Add(new SegmentRevision { SegmentId = entity.Id, Version = 1, Json = entity.Json, CreatedBy = createdBy });
            await _db.SaveChangesAsync();
            return Ok(new { entity.Id, entity.Name, entity.Version });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<object>> Update(Guid id, [FromBody] SegmentDefinition def, [FromQuery] string updatedBy = "ui")
        {
            var entity = await _db.Segments.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null) return NotFound();
            entity.Name = def.Name;
            entity.Version += 1;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            entity.Json = JsonSerializer.Serialize(def, _json);
            _db.SegmentRevisions.Add(new SegmentRevision { SegmentId = entity.Id, Version = entity.Version, Json = entity.Json, CreatedBy = updatedBy });
            await _db.SaveChangesAsync();
            return Ok(new { entity.Id, entity.Name, entity.Version });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<object>> Get(Guid id)
        {
            var entity = await _db.Segments.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpGet("list")]
        public async Task<ActionResult<object>> List()
        {
            var list = await _db.Segments.OrderByDescending(s => s.UpdatedAtUtc).ToListAsync();
            return Ok(list);
        }

        [HttpPost("{id:guid}/queue-braze")]
        public async Task<ActionResult<object>> QueueBraze(Guid id, [FromQuery] string mode, [FromQuery] string key, [FromQuery] string action = "add")
        {
            var entity = await _db.Segments.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null) return NotFound();

            var type = (mode, action.ToLower()) switch
            {
                ("flag", "add")    => BrazeJobType.AddFlag,
                ("flag", "remove") => BrazeJobType.RemoveFlag,
                ("array", "add")   => BrazeJobType.AddCohortId,
                ("array", "remove")=> BrazeJobType.RemoveCohortId,
                _ => throw new ArgumentException("Invalid mode/action")
            };

            _db.BrazeJobs.Add(new BrazeJob { Type = type, Key = key, SegmentJson = entity.Json });
            await _db.SaveChangesAsync();
            return Accepted(new { Queued = true });
        }
    }
}
