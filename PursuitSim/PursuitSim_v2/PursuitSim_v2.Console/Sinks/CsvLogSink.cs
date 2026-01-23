using PursuitSim_v2.Core.Logging;

public class CsvLogSink
{
    private readonly CsvExporter _exporter;

    public CsvLogSink(string outDir, string scenarioName, string scenarioId)
    {
        _exporter = new CsvExporter(outDir, scenarioName, scenarioId, DateTime.Now);
    }

    public void Write(string message)
    {
        _exporter.WriteRaw(message);
    }

    public void Close()
    {
        _exporter.Close();
    }
}
