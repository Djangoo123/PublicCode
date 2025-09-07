using PursuitSim.Model;

namespace PursuitSim.UI;

public static class Menu
{
    public static (Scenario scenario, string scenarioId, int soldiers, bool batchMode, int runs) AskUser()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // --- Scenario selection ---
        Console.WriteLine("Choose a scenario:");
        Console.WriteLine("  1) Plain + Hedges");
        Console.WriteLine("  2) Urban Grid");
        Console.WriteLine("  3) Mixed Clearing");
        Console.Write("Your choice [1-3] (default = 1): ");
        var scenarioInput = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(scenarioInput)) scenarioInput = "1";

        Scenario s;
        string scenarioId = scenarioInput;
        switch (scenarioInput)
        {
            case "2": s = Scenarios.UrbanGrid(); break;
            case "3": s = Scenarios.MixedClearingFinal(); break;
            default: s = Scenarios.PlainWithHedges(); scenarioId = "1"; break;
        }

        // --- Team size ---
        Console.Write("How many soldiers in the team? (default = 3): ");
        var soldiersInput = Console.ReadLine()?.Trim();
        int soldierCount = 3;
        if (!string.IsNullOrEmpty(soldiersInput) && int.TryParse(soldiersInput, out var n) && n > 0)
            soldierCount = n;
        s.Team.Count = soldierCount;

        Console.WriteLine($"→ Team configured with {soldierCount} soldier(s).");
        Console.WriteLine();

        // --- Mode selection ---
        Console.WriteLine("Simulation mode:");
        Console.WriteLine("  1) Single run");
        Console.WriteLine("  2) Multiple runs (batch)");
        Console.Write("Your choice [1-2] (default = 1): ");
        var modeInput = Console.ReadLine()?.Trim();
        bool batchMode = (modeInput == "2");

        int runs = 1;
        if (batchMode)
        {
            Console.Write("How many runs? (default = 10): ");
            var runsInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(runsInput) && int.TryParse(runsInput, out var r) && r > 0)
                runs = r;
            else
                runs = 10;
        }

        return (s, scenarioId, soldierCount, batchMode, runs);
    }
}
