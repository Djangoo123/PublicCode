using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.Grafana.Loki.Labels;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var lokiUri = configuration["Loki:Uri"];
var labelsConfig = configuration.GetSection("Loki:Labels").Get<List<LokiLabelConfig>>();

var labelsBuilder = new LokiLabelsBuilder();
foreach (var item in labelsConfig)
{
    labelsBuilder.Add(item.Key, item.Value);
}
var labels = labelsBuilder.Build();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(
        uri: lokiUri,
        credentials: new LokiCredentials
        {
            Login = configuration["Loki:Credentials:Login"],
            Password = configuration["Loki:Credentials:Password"]
        },
        labels: labels
    )
    .CreateLogger();

try
{
    Log.Information("Hello Grafana Loki with config! {@Details}", new { Message = "Test via appsettings.json", Date = DateTime.UtcNow });
}
catch (Exception ex)
{
    Log.Error(ex, "Error found");
}
finally
{
    Log.CloseAndFlush();
}

public class LokiLabelConfig
{
    public string Key { get; set; }
    public string Value { get; set; }
}
