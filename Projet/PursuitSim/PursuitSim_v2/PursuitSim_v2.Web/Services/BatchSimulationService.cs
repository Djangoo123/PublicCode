using PursuitSim_v2.ConsoleApp.Engine;
using PursuitSim_v2.Core.DroneSim;

namespace PursuitSim_v2.Web.Services
{
    public sealed class BatchSimulationService
    {
        public async Task<List<RunResult>> RunBatchAsync(
            Func<Scenario> scenarioFactory,
            int runs,
            string scenarioId,
            string outDir)
        {
            return await Task.Run(() =>
            {
                var scenario = scenarioFactory();
                return BatchRunner.Run(scenario, scenarioId, outDir, runs);
            });
        }
    }
}
