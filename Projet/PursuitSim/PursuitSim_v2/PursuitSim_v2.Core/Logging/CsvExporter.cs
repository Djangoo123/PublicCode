using System.Globalization;
using System.Text;
using PursuitSim_v2.Core.DroneSim;
using PursuitSim_v2.Core.DroneSim.Models;

namespace PursuitSim_v2.Core.Logging;

public sealed class CsvExporter : IDisposable
{
    readonly string _baseOutDir;
    readonly string _scenarioName;
    readonly string _scenarioId;
    readonly DateTime _startLocal;
    readonly CultureInfo _ci = CultureInfo.InvariantCulture;

    string _dir = string.Empty;
    string _tmpPath = string.Empty;
    string _finalPath = string.Empty;
    StreamWriter? _sw;
    bool _headerWritten = false;
    int _runnerCols = 0;

    public string FinalPath => _finalPath;

    public CsvExporter(string baseOutDir, string scenarioName, string scenarioId, DateTime startLocal)
    {
        _baseOutDir = string.IsNullOrWhiteSpace(baseOutDir) ? "out" : baseOutDir;
        _scenarioName = Sanitize(scenarioName);
        _scenarioId = scenarioId;
        _startLocal = startLocal;
        Prepare();
    }

    static string Sanitize(string s)
    {
        var bad = Path.GetInvalidFileNameChars();
        var cleaned = new string(s.Select(ch => bad.Contains(ch) ? '_' : ch).ToArray());
        return cleaned.Replace(' ', '_');
    }

    void Prepare()
    {
        var day = _startLocal.ToString("yyyy-MM-dd");
        _dir = Path.Combine(_baseOutDir, day, _scenarioName);
        Directory.CreateDirectory(_dir);

        var stamp = _startLocal.ToString("yyyy-MM-dd_HH-mm-ss");
        var baseName = $"run_{_scenarioId}_{stamp}_dt1s.csv";
        _finalPath = Path.Combine(_dir, baseName);
        _tmpPath = _finalPath + ".tmp";

        // UTF8 with BOM for Excel friendliness
        _sw = new StreamWriter(new FileStream(_tmpPath, FileMode.Create, FileAccess.Write, FileShare.Read),
                               new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    void WriteHeaderIfNeeded(Team team)
    {
        if (_headerWritten || _sw is null) return;
        _runnerCols = team.Runners.Length;

        var cols = new[]
        {
            "time_s","scenario","team_state","alive","path","headY_m",
            "drone_state","drone_x_m","drone_y_m","event_type","event_note"
        }.ToList();

        for (int i = 0; i < _runnerCols; i++)
        {
            cols.Add($"r{i + 1}_x");
            cols.Add($"r{i + 1}_y");
            cols.Add($"r{i + 1}_ko");
        }

        _sw.WriteLine(string.Join(",", cols));
        _headerWritten = true;
    }

    static string PathTag(Scenario s, Team team)
    {
        return ReferenceEquals(team.CurrentPath, s.MainPath) ? "Main" :
               ReferenceEquals(team.CurrentPath, s.AltPathA) ? "AltA" :
               ReferenceEquals(team.CurrentPath, s.AltPathB) ? "AltB" : "Other";
    }

    public void WriteTick(double t, Scenario s, Team team, Drone drone)
    {
        if (_sw is null) return;
        WriteHeaderIfNeeded(team);

        var fields = new[]
        {
            t.ToString("0.##", _ci),
            _scenarioName,
            team.State.ToString(),
            team.AliveCount.ToString(_ci),
            PathTag(s, team),
            team.HeadY.ToString("0.###", _ci),
            drone.State.ToString(),
            drone.Pos.X.ToString("0.###", _ci),
            drone.Pos.Y.ToString("0.###", _ci),
            string.Empty, // event_type
            string.Empty  // event_note
        }.ToList();

        // runners
        foreach (var r in team.Runners)
        {
            fields.Add(r.Pos.X.ToString("0.###", _ci));
            fields.Add(r.Pos.Y.ToString("0.###", _ci));
            fields.Add(r.KO ? "1" : "0");
        }

        _sw.WriteLine(string.Join(",", fields));
    }

    public void WriteEvent(double t, Scenario s, Team team, Drone drone, string eventType, string? note = null)
    {
        if (_sw is null) return;
        WriteHeaderIfNeeded(team);

        var fields = new[]
        {
            t.ToString("0.##", _ci),
            _scenarioName,
            team.State.ToString(),
            team.AliveCount.ToString(_ci),
            PathTag(s, team),
            team.HeadY.ToString("0.###", _ci),
            drone.State.ToString(),
            drone.Pos.X.ToString("0.###", _ci),
            drone.Pos.Y.ToString("0.###", _ci),
            eventType,
            note ?? string.Empty
        }.ToList();

        for (int i = 0; i < _runnerCols; i++)
        {
            var r = i < team.Runners.Length ? team.Runners[i] : null;
            fields.Add((r?.Pos.X ?? 0).ToString("0.###", _ci));
            fields.Add((r?.Pos.Y ?? 0).ToString("0.###", _ci));
            fields.Add(r != null && r.KO ? "1" : "0");
        }

        _sw.WriteLine(string.Join(",", fields));
    }

    public void Close()
    {
        if (_sw is null) return;
        _sw.Flush();
        _sw.Dispose();
        _sw = null;

        if (File.Exists(_finalPath)) File.Delete(_finalPath);
        File.Move(_tmpPath, _finalPath);
    }

    public void WriteRaw(string line)
    {
        _sw?.WriteLine(line);
    }

    public void Dispose() => Close();
}
