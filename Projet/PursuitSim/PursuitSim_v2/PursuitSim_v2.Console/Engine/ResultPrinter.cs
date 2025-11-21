using PursuitSim_v2.Core.Common;
using PursuitSim_v2.Core.DroneSim;

namespace PursuitSim_v2.ConsoleApp.Engine;

public static class ResultPrinter
{
    public static void PrintSingle(Simulation sim, Scenario scenario)
    {
        global::System.Console.WriteLine();
        global::System.Console.WriteLine("=== Simulation finished ===");
        global::System.Console.WriteLine($"Scenario : {scenario.Name}");
        global::System.Console.WriteLine($"Soldiers : {scenario.Team.Count}");

        if (sim.Team.State == TeamState.Win)
        {
            global::System.Console.WriteLine($"Result   : WIN, Survivors={sim.Team.AliveCount}, Duration={sim.ElapsedTime:F1}s");
        }
        else if (sim.Team.State == TeamState.Fail)
        {
            global::System.Console.WriteLine($"Result   : FAIL ({sim.FailReason}), Survivors={sim.Team.AliveCount}, Duration={sim.ElapsedTime:F1}s");
        }
        else
        {
            global::System. Console.WriteLine($"Result   : {sim.Team.State}, Survivors={sim.Team.AliveCount}, Duration={sim.ElapsedTime:F1}s");
        }
    }


    public static void PrintBatch(List<RunResult> results, Scenario scenario)
    {
        global::System.Console.WriteLine();
        global::System.Console.WriteLine("=== Batch Results ===");
        global::System.Console.WriteLine($"Scenario : {scenario.Name}");
        global::System.Console.WriteLine($"Soldiers : {scenario.Team.Count}");

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

        global::System.Console.WriteLine($"Total runs     : {results.Count}");
        global::System.Console.WriteLine($"Win rate       : {winRate:F1}%");
        global::System.Console.WriteLine($"Avg survivors  : {results.Average(r => r.Survivors):F2}");
        global::System.Console.WriteLine($"Best result    : {(best.Win ? "WIN" : "FAIL")}, Survivors={best.Survivors}, Duration={best.Duration:F1}s");
        global::System.Console.WriteLine($"Worst result   : {(worst.Win ? "WIN" : "FAIL")}, Survivors={worst.Survivors}, Duration={worst.Duration:F1}s"
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
        global::System.Console.WriteLine($"Batch summary exported to {path}");
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

        global::System.Console.WriteLine($"Single summary exported to {path}");
    }

    public static void PrintUrbanSiegeBatch(List<UrbanSiegeRunResult> results)
    {
        global::System.Console.WriteLine();
        global::System.Console.WriteLine("=== Urban Siege – Batch Results ===");

        int total = results.Count;
        int attackersWins = results.Count(r => r.AttackersWin);
        double winRate = (double)attackersWins / total * 100.0;

        global::System.Console.WriteLine($"Total runs       : {total}");
        global::System.Console.WriteLine($"Attackers winrate: {winRate:F1}%");

        var best = results
            .OrderByDescending(r => r.AliveAttackers)
            .ThenByDescending(r => r.AliveDefenders)
            .First();

        var worst = results
            .OrderBy(r => r.AliveAttackers)
            .ThenBy(r => r.AliveDefenders)
            .First();

        global::System.Console.WriteLine();
        global::System.Console.WriteLine($"Best run         : #{best.RunIndex}");
        global::System.Console.WriteLine($"  Result         : {best.ResultText}");
        global::System.Console.WriteLine($"  Attackers alive: {best.AliveAttackers}");
        global::System.Console.WriteLine($"  Defenders alive: {best.AliveDefenders}");

        global::System.Console.WriteLine();
        global::System.Console.WriteLine($"Worst run        : #{worst.RunIndex}");
        global::System.Console.WriteLine($"  Result         : {worst.ResultText}");
        global::System.Console.WriteLine($"  Attackers alive: {worst.AliveAttackers}");
        global::System.Console.WriteLine($"  Defenders alive: {worst.AliveDefenders}");
    }


    public static void ExportUrbanSiegeBatchCsv(List<UrbanSiegeRunResult> results, string outDir)
    {
        var dateDir = DateTime.Now.ToString("yyyy-MM-dd");
        var dir = Path.Combine(outDir, dateDir, "UrbanSiege");
        Directory.CreateDirectory(dir);

        var fileName = $"urban_batch_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
        var path = Path.Combine(dir, fileName);

        using var sw = new StreamWriter(path, false, System.Text.Encoding.UTF8);

        sw.WriteLine("run,attackers_win,alive_attackers,alive_defenders,result_text");

        foreach (var r in results)
        {
            sw.WriteLine(
                $"{r.RunIndex}," +
                $"{(r.AttackersWin ? "WIN" : "FAIL")}," +
                $"{r.AliveAttackers}," +
                $"{r.AliveDefenders}," +
                $"{r.ResultText.Replace(",", ";")}"
            );
        }

        sw.Flush();
        global::System.Console.WriteLine($"Urban Siege batch CSV exported to {path}");
    }



}
