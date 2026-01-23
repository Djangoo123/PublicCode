using PursuitSim_v2.Console.Config;
using PursuitSim_v2.ConsoleApp.Engine;
using PursuitSim_v2.Core.DroneSim;
using PursuitSim_v2.Core.Logging;
using PursuitSim_v2.Core.UrbanSiege;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Load config
string outDir = ConfigLoader.GetOutputDir("appsettings.json");

while (true)
{
    Console.Clear();
    Console.WriteLine("=== PursuitSim_v2 Console ===");
    Console.WriteLine("1. DroneSim – Single Run");
    Console.WriteLine("2. DroneSim – Batch Run");
    Console.WriteLine("3. Urban Siege – Single Run");
    Console.WriteLine("4. Urban Siege – Batch Run");  
    Console.WriteLine("5. Exit");
    Console.Write("Select option: ");

    var key = Console.ReadKey().KeyChar;
    Console.WriteLine("\n");

    switch (key)
    {
        case '1': RunDroneSimSingle(outDir); break;
        case '2': RunDroneSimBatch(); break;
        case '3': RunUrbanSiege(outDir); break;
        case '4': RunUrbanSiegeBatch(outDir); break;
        case '5': return;
        default: Console.WriteLine("Invalid option."); break;
    }

    Console.WriteLine("\nPress any key to return to menu...");
    Console.ReadKey(true);
}


static void RunDroneSimSingle(string outDir)
{
    Console.WriteLine("Available DroneSim scenarios:");
    Console.WriteLine("1. Plain + Hedges");
    Console.WriteLine("2. Urban Grid");
    Console.WriteLine("3. Mixed Clearing Final");
    Console.Write("Choose scenario: ");

    var choice = Console.ReadKey().KeyChar;
    Console.WriteLine("\n");

    // === BATCH MODE
    if (choice == 'B' || choice == 'b')
    {
        RunDroneSimBatch();
        return;
    }


    Scenario? scenario = choice switch
    {
        '1' => Scenarios.PlainWithHedges(),
        '2' => Scenarios.UrbanGrid(),
        '3' => Scenarios.MixedClearingFinal(),
        _ => null
    };

    if (scenario == null)
    {
        Console.WriteLine("Invalid scenario.");
        return;
    }

    Console.Write("Scenario ID: ");
    var scenarioId = Console.ReadLine() ?? "run";

    var sim = new Simulation(scenario, scenarioId);

    var csv = new CsvExporter(outDir, scenario.Name, scenarioId, DateTime.Now);

    sim.LogMessage += Console.WriteLine;
    sim.LogMessage += csv.WriteRaw;

    sim.Run();
    csv.Close();

    ResultPrinter.PrintSingle(sim, scenario);
    ResultPrinter.ExportSingleCsv(sim, scenario, outDir);

    Console.WriteLine("=== DroneSim run complete ===");
}

static void RunUrbanSiege(string outDir)
{
    Console.WriteLine("Running Urban Siege scenario...");
    var scenario = UrbanSiegeFactory.Create();
    var engine = new UrbanSiegeEngine(scenario);

    var csv = new CsvExporter(outDir, scenario.Name, "urban", DateTime.Now);

    engine.Log += Console.WriteLine;
    engine.Log += csv.WriteRaw;

    engine.Run();
    csv.Close();

    Console.WriteLine("=== Urban Siege run complete ===");
}

static void RunDroneSimBatch()
{
    Console.WriteLine("Batch mode selected.");

    Console.WriteLine("Choose scenario:");
    Console.WriteLine("1. Plain + Hedges");
    Console.WriteLine("2. Urban Grid");
    Console.WriteLine("3. Mixed Clearing Final");
    Console.Write("Scenario: ");

    var choice = Console.ReadKey().KeyChar;
    Console.WriteLine("\n");

    Scenario? scenario = choice switch
    {
        '1' => Scenarios.PlainWithHedges(),
        '2' => Scenarios.UrbanGrid(),
        '3' => Scenarios.MixedClearingFinal(),
        _ => null
    };

    if (scenario == null)
    {
        Console.WriteLine("Invalid scenario.");
        return;
    }

    Console.Write("Scenario ID: ");
    var scenarioId = Console.ReadLine() ?? "batch";

    Console.Write("Number of runs: ");
    int runs = int.TryParse(Console.ReadLine(), out var r) ? r : 10;

    var outDir = ConfigLoader.GetOutputDir("appsettings.json");

    var results = BatchRunner.Run(scenario, scenarioId, outDir, runs);

    ResultPrinter.PrintBatch(results, scenario);

    ResultPrinter.ExportBatchCsv(results, scenario, outDir);

    Console.WriteLine("=== Batch mode complete ===");
}

static void RunUrbanSiegeBatch(string outDir)
{
    Console.WriteLine("=== Urban Siege – Batch Mode ===");

    Console.Write("Number of runs: ");
    int runs = int.TryParse(Console.ReadLine(), out var r) ? r : 10;

    Console.WriteLine($"Running {runs} simulations...\n");

    var results = UrbanSiegeBatchRunner.Run(runs);

    ResultPrinter.PrintUrbanSiegeBatch(results);

    ResultPrinter.ExportUrbanSiegeBatchCsv(results, outDir);

    Console.WriteLine("\n=== Urban Siege batch complete ===");
}


