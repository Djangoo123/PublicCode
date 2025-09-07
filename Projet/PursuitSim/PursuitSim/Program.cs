using PursuitSim.Engine;
using PursuitSim.UI;

namespace PursuitSim;

class Program
{
    static void Main(string[] args)
    {
        var (scenario, scenarioId, soldiers, batchMode, runs) = Menu.AskUser();
        var outDir = ConfigLoader.GetOutputDir("appsettings.json");

        if (batchMode)
        {
            var results = BatchRunner.Run(scenario, scenarioId, outDir, runs);
            ResultPrinter.PrintBatch(results, scenario);
        }
        else
        {
            var sim = new Simulation(scenario, scenarioId, outDir);
            sim.Run();
            ResultPrinter.PrintSingle(sim, scenario);
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey(true);
    }
}
