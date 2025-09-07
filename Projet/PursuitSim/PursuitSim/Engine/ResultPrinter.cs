using PursuitSim.Core;
using PursuitSim.Model;

namespace PursuitSim.Engine;

public static class ResultPrinter
{
    public static void PrintSingle(Simulation sim, Scenario scenario)
    {
        Console.WriteLine();
        Console.WriteLine("=== Simulation finished ===");
        Console.WriteLine($"Scenario : {scenario.Name}");
        Console.WriteLine($"Soldiers : {scenario.Team.Count}");

        if (sim.Team.State == TeamState.Win)
        {
            Console.WriteLine($"Result   : WIN, Survivors={sim.Team.AliveCount}, Duration={sim.ElapsedTime:F1}s");
        }
        else if (sim.Team.State == TeamState.Fail)
        {
            Console.WriteLine($"Result   : FAIL ({sim.FailReason}), Survivors={sim.Team.AliveCount}, Duration={sim.ElapsedTime:F1}s");
        }
        else
        {
            Console.WriteLine($"Result   : {sim.Team.State}, Survivors={sim.Team.AliveCount}, Duration={sim.ElapsedTime:F1}s");
        }
    }


    public static void PrintBatch(List<RunResult> results, Scenario scenario)
    {
        Console.WriteLine();
        Console.WriteLine("=== Batch Results ===");
        Console.WriteLine($"Scenario : {scenario.Name}");
        Console.WriteLine($"Soldiers : {scenario.Team.Count}");

        int totalWins = results.Count(r => r.Win);
        double winRate = (double)totalWins / results.Count * 100.0;

        var best = results
            .OrderByDescending(r => r.Survivors)
            .ThenBy(r => r.Duration)
            .First();

        var worst = results
            .OrderBy(r => r.Survivors)
            .ThenBy(r => r.Duration)
            .First();

        Console.WriteLine($"Total runs     : {results.Count}");
        Console.WriteLine($"Win rate       : {winRate:F1}%");
        Console.WriteLine($"Avg survivors  : {results.Average(r => r.Survivors):F2}");
        Console.WriteLine($"Best result    : {(best.Win ? "WIN" : "FAIL")}, Survivors={best.Survivors}, Duration={best.Duration:F1}s");
        Console.WriteLine($"Worst result   : {(worst.Win ? "WIN" : "FAIL")}, Survivors={worst.Survivors}, Duration={worst.Duration:F1}s"
            + (string.IsNullOrEmpty(worst.FailReason) ? "" : $", Reason={worst.FailReason}"));
    }


}
