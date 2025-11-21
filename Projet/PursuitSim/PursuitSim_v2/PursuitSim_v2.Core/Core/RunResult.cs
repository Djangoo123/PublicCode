namespace PursuitSim_v2.Model;

public class RunResult
{
    public bool Win { get; set; }
    public int Survivors { get; set; }
    public double Duration { get; set; }
    public string FailReason { get; set; } = string.Empty;
    public int RunIndex { get; set; }    
}
