namespace PursuitSim_v2.Core.DroneSim.Models
{
    public sealed class DroneParams
    {
        public double PatrolSpeed = 16.7;     // m/s ~ 60 km/h
        public double ChaseSpeed = 16.7;      // m/s (55.6 for "hard mode")
        public double DetectionRange = 200.0; // m
        public double AttackRadius = 3.0;     // m
        public double ReactLatency = 0.5;     // s
        public double LoseSightAfter = 3.0;   // s sans LoS
        public double RespawnSeconds = 300.0; // 5 min
        public double FovCosine = Math.Cos(Math.PI / 2.0); // 90° de FOV
    }
}
