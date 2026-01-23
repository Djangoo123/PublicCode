namespace PursuitSim_v2.Console.Config;

public static class ConfigLoader
{
    public static string GetOutputDir(string configFile)
    {
        var outDir = "out";

        if (File.Exists(configFile))
        {
            var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(configFile));
            if (json.RootElement.TryGetProperty("OutputDir", out var prop))
                outDir = prop.GetString() ?? "out";
        }

        return outDir;
    }
}
