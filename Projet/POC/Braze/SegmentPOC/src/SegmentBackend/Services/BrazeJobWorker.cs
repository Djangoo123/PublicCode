
using Microsoft.EntityFrameworkCore;
using SegmentBackend.Data;
using SegmentBackend.Models;

namespace SegmentBackend.Services
{
    public sealed class BrazeJobWorker : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<BrazeJobWorker> _log;

        public BrazeJobWorker(IServiceProvider sp, ILogger<BrazeJobWorker> log)
        {
            _sp = sp; _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var braze = scope.ServiceProvider.GetService<BrazeClient>();

                    var job = await db.BrazeJobs.Where(j => j.Status == BrazeJobStatus.Pending)
                                                .OrderBy(j => j.CreatedAtUtc)
                                                .FirstOrDefaultAsync(stoppingToken);
                    if (job is null)
                    {
                        await Task.Delay(1000, stoppingToken);
                        continue;
                    }

                    job.Status = BrazeJobStatus.Running;
                    job.Attempts++;
                    await db.SaveChangesAsync(stoppingToken);

                    var run = new BrazeJobRun { JobId = job.Id, StartedAtUtc = DateTime.UtcNow };
                    db.BrazeJobRuns.Add(run);
                    await db.SaveChangesAsync(stoppingToken);

                    if (braze is null) throw new InvalidOperationException("Braze client not configured");

                    var def = System.Text.Json.JsonSerializer.Deserialize<SegmentDefinition>(job.SegmentJson, new System.Text.Json.JsonSerializerOptions{ PropertyNameCaseInsensitive = true })!;
                    var pred = SegmentToExpression.BuildPredicate(def.Root);
                    var ids = await db.Users.AsNoTracking().Where(pred).Select(u => u.Id).ToListAsync(stoppingToken);

                    int okBatches = 0;
                    foreach (var chunk in ids.Chunk(75))
                    {
                        var attrs = chunk.Select(id => BuildAttributes(job, id)).ToList();
                        var (ok, errors) = await braze.UpsertAsync(attrs, stoppingToken);
                        okBatches += ok;
                        foreach (var err in errors)
                            db.BrazeSyncLogs.Add(new BrazeSyncLog { Action = $"Braze {job.Type}", PayloadSummary = $"{chunk.Length} users", Error = err });
                    }

                    job.Status = BrazeJobStatus.Succeeded;
                    run.Success = true;
                    run.Result = $"BatchesOk={okBatches}, Users={ids.Count}";
                    run.FinishedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var lastJob = await db.BrazeJobs.Where(j => j.Status == BrazeJobStatus.Running).OrderByDescending(j => j.CreatedAtUtc).FirstOrDefaultAsync(stoppingToken);
                    if (lastJob != null)
                    {
                        lastJob.Status = lastJob.Attempts >= 3 ? BrazeJobStatus.Failed : BrazeJobStatus.Pending;
                        lastJob.Error = ex.Message;
                        await db.SaveChangesAsync(stoppingToken);
                    }
                    _log.LogError(ex, "Braze job failed");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private static Dictionary<string, object> BuildAttributes(BrazeJob job, string externalId)
        {
            var dict = new Dictionary<string, object> { ["external_id"] = externalId };
            switch (job.Type)
            {
                case BrazeJobType.AddFlag: dict[job.Key] = true; break;
                case BrazeJobType.RemoveFlag: dict[job.Key] = false; break;
                case BrazeJobType.AddCohortId:
                    dict["cohort_ids"] = new Dictionary<string, object>{ ["add"] = new [] { job.Key } }; break;
                case BrazeJobType.RemoveCohortId:
                    dict["cohort_ids"] = new Dictionary<string, object>{ ["remove"] = new [] { job.Key } }; break;
            }
            return dict;
        }
    }
}
