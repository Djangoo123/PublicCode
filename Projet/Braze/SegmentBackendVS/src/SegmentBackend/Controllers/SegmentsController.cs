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
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SegmentsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Preview: renvoie Count + Sample pour une définition de segment JSON.
        /// </summary>
        [HttpPost("preview")]
        public async Task<ActionResult<object>> Preview([FromBody] JsonElement segmentJson, [FromQuery] int take = 25, CancellationToken ct = default)
        {
            var def = JsonSerializer.Deserialize<SegmentDefinition>(segmentJson.GetRawText(), _json)!;
            var predicate = SegmentToExpression.BuildPredicate(def.Root);

            var query = _db.Users.AsNoTracking().Where(predicate);
            var count = await query.CountAsync(ct);
            var sample = await query.Take(Math.Clamp(take, 1, 200))
                                    .Select(u => new { u.Id, u.Country, u.Plan, u.OrdersCount, u.TotalSpend, u.CreatedUtc })
                                    .ToListAsync(ct);

            return Ok(new { Count = count, Sample = sample });
        }

        /// <summary>
        /// Filtre les users selon la définition et pousse un attribut booléen dans Braze pour chacun.
        /// Query: tag (obligatoire) -> sera utilisé comme nom d'attribut 'cohort_{tag}' = true.
        /// </summary>
        [HttpPost("push-to-braze")]
        public async Task<ActionResult<object>> PushToBraze(
            [FromBody] JsonElement segmentJson,
            [FromQuery] string tag,
            [FromServices] SegmentBackend.Services.BrazeClient? braze,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(tag)) return BadRequest("Query parameter 'tag' is required.");
            if (braze is null) return StatusCode(500, "Braze client is not configured. Set BRAZE_REST_ENDPOINT and BRAZE_REST_API_KEY or appsettings Braze:Endpoint/ApiKey.");

            var def = JsonSerializer.Deserialize<SegmentDefinition>(segmentJson.GetRawText(), _json)!;
            var predicate = SegmentToExpression.BuildPredicate(def.Root);

            var cohortKey = $"cohort_{tag}".Replace("-", "_").Replace(" ", "_");
            var ids = await _db.Users.AsNoTracking()
                                     .Where(predicate)
                                     .Select(u => u.Id)
                                     .ToListAsync(ct);

            var res = await braze.UpsertCohortAsync(ids, cohortKey, true, ct);
            return Ok(new { Matched = ids.Count, CohortKey = cohortKey, BatchesOk = res.SuccessfulBatches, BatchFailures = res.Failures });
        }
    }
}