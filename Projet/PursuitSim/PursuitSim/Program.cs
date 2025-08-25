
using PursuitSim.Core;
using PursuitSim.Engine;
using PursuitSim.Model;

namespace PursuitSim;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var scenario = (args.Length > 0 ? args[0] : "1").Trim();
        Scenario s = scenario switch
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

        var sim = new Simulation(s);
        sim.Run();
    }
}
