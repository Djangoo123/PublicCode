using PursuitSim.Core;
using PursuitSim.Model;

namespace PursuitSim.Engine;

public static class BatchRunner
{
    public static List<RunResult> Run(Scenario scenario, string scenarioId, string outDir, int runs)
    {
        var results = new List<RunResult>();
        for (int i = 0; i < runs; i++)
        {
            var sim = new Simulation(scenario, scenarioId, outDir);
            sim.Run();

            results.Add(new RunResult
            {
                Win = sim.Team.State == TeamState.Win,
                Survivors = sim.Team.AliveCount,
                Duration = sim.ElapsedTime
            });
        }
        return results;
    }
}
