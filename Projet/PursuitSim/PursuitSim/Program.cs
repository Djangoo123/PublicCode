using PursuitSim.Core;
using PursuitSim.Engine;
using PursuitSim.Model;

namespace PursuitSim;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var scenarioId = (args.Length > 0 ? args[0] : "1").Trim();

        Scenario s = scenarioId switch
        {
            "1" => Scenarios.PlainWithHedges(),
            "2" => Scenarios.UrbanGrid(),
            "3" => Scenarios.MixedClearingFinal(),
            _ => Scenarios.PlainWithHedges()
        };

        Console.WriteLine("=== PursuitSim (.NET 8) ===");
        Console.WriteLine($"Scenario: {s.Name}");
        Console.WriteLine("Args: 1=Plain+Hedges, 2=Urban Grid, 3=Mixed Clearing");
        Console.WriteLine();

        var outDir = "out";
        if (File.Exists("appsettings.json"))
        {
            var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText("appsettings.json"));
            if (json.RootElement.TryGetProperty("OutputDir", out var prop))
                outDir = prop.GetString() ?? "out";
        }
        var sim = new Simulation(s, scenarioId, outDir);

        sim.Run();
    }
}
