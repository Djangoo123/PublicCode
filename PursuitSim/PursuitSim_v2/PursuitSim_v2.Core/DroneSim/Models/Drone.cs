using PursuitSim_v2.Core.Common;

namespace PursuitSim_v2.Core.DroneSim.Models
{
    public class Drone
    {
        public DroneParams P;
        public DroneState State = DroneState.Patrol;
        public Vec2 Pos;
        public Vec2 Vel;
        public double TimeSinceLoS = 0;
        public double Cooldown = 0;
        public int PatrolLane = 0;
        public bool HasTarget => Target != null;
        public Runner? Target;
        public double ReactTimer = 0;

        public DroneType Type = DroneType.Hunter;
        public double BombAltitude = 100.0;  // meters
        public double BombTimer = 0.0;       // countdown until impact

        public Drone(DroneParams p) { P = p; Pos = new Vec2(50, 50); }

        public void ResetForPatrol()
        {
            State = DroneState.Patrol;
            PatrolLane = 0;
            Pos = new Vec2(50, 50);
            Vel = new Vec2(P.PatrolSpeed, 0);
            TimeSinceLoS = 0;
            Cooldown = 0;
            Target = null;
            ReactTimer = 0;

            // Randomly choose type: 50% Hunter, 50% Bomber
            if (new Random().NextDouble() < 0.5)
                Type = DroneType.Hunter;
            else
                Type = DroneType.Bomber;

            BombTimer = 0;

        }
    }

}
