namespace PursuitSim_v2.ConsoleApp.Engine
{
    public class RunResult
    {
        public int RunIndex { get; set; }
        public bool Win { get; set; }
        public int Survivors { get; set; }
        public double Duration { get; set; }
        public string FailReason { get; set; } = "";
    }

}
