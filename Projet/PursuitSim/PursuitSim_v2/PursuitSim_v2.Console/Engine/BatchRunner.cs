using PursuitSim_v2.Core.Common;
using PursuitSim_v2.Core.DroneSim;
using PursuitSim_v2.Model;

namespace PursuitSim_v2.ConsoleApp.Engine;

public static class BatchRunner
{
    public static List<RunResult> Run(Scenario scenario, string scenarioId, string outDir, int runs)
    {
        var results = new List<RunResult>();

        for (int i = 0; i < runs; i++)
        {
            var sim = new Simulation(scenario, scenarioId);

            sim.Run();

            results.Add(new RunResult
            {
                RunIndex = i + 1,
                Win = sim.Team.State == TeamState.Win,
                Survivors = sim.Team.AliveCount,
                Duration = sim.ElapsedTime,
                FailReason = sim.Team.State == TeamState.Fail ? sim.FailReason : ""
            });
        }

        return results;
    }
}
