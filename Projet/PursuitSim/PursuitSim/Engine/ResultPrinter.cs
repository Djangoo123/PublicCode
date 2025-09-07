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

    public static void ExportBatchCsv(List<RunResult> results, Scenario scenario, string outDir)
    {
        var dateDir = DateTime.Now.ToString("yyyy-MM-dd");
        var dir = Path.Combine(outDir, dateDir, scenario.Name.Replace(" ", "_"));
        Directory.CreateDirectory(dir);

        var fileName = $"batch_summary_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
        var path = Path.Combine(dir, fileName);

        using var sw = new StreamWriter(path, false, System.Text.Encoding.UTF8);

        // Header
        sw.WriteLine("run,scenario,soldiers,result,survivors,duration_s,fail_reason");

        // Rows
        foreach (var r in results)
        {
            var result = r.Win ? "WIN" : "FAIL";
            var fail = string.IsNullOrEmpty(r.FailReason) ? "" : r.FailReason.Replace(",", ";");
            sw.WriteLine($"{r.RunIndex},{scenario.Name},{scenario.Team.Count},{result},{r.Survivors},{r.Duration:F1},{fail}");
        }

        sw.Flush();
        Console.WriteLine($"Batch summary exported to {path}");
    }

    public static void ExportSingleCsv(Simulation sim, Scenario scenario, string outDir)
    {
        var dateDir = DateTime.Now.ToString("yyyy-MM-dd");
        var dir = Path.Combine(outDir, dateDir, scenario.Name.Replace(" ", "_"));
        Directory.CreateDirectory(dir);

        var fileName = $"single_summary_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
        var path = Path.Combine(dir, fileName);

        using var sw = new StreamWriter(path, false, System.Text.Encoding.UTF8);
        sw.WriteLine("scenario,soldiers,result,survivors,duration_s,fail_reason");

        var result = sim.Team.State == TeamState.Win ? "WIN" : "FAIL";
        var fail = sim.Team.State == TeamState.Fail ? sim.FailReason : "";

        sw.WriteLine($"{scenario.Name},{scenario.Team.Count},{result},{sim.Team.AliveCount},{sim.ElapsedTime:F1},{fail}");
        sw.Flush();

        Console.WriteLine($"Single summary exported to {path}");
    }

}
