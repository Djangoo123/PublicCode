
using PursuitSim.Core;

namespace PursuitSim.Model;

public enum TeamState { Stealth, SwitchPath, Evasion, ContinueWith2, Retreat, Win, Fail }
public enum DroneState { Patrol, Track, Attack, Bombing, Destroyed, Cooldown }
public enum DroneType { Hunter, Bomber }
public sealed class Runner
{
    public bool KO;
    public double S; // distance along current path
    public Vec2 Pos;
}

public sealed class Team
{
    public TeamParams P;
    public TeamState State = TeamState.Stealth;
    public Runner[] Runners;
    public PolyPath CurrentPath;
    public PolyPath? TargetAltPath;
    public double TargetAccessY = -1;
    public double LastCheckpointY = 0;

    public Team(TeamParams p, PolyPath startPath)
    {
        P = p;
        Runners = new Runner[P.Count];
        CurrentPath = startPath;
        for (int i = 0; i < P.Count; i++)
        {
            Runners[i] = new Runner { KO = false, S = -i * P.Spacing, Pos = startPath.GetPointAtDistance(0) };
        }
    }

    public int AliveCount => Runners.Count(r => !r.KO);

    public double HeadY => Runners.Where(r => !r.KO).Select(r => r.Pos.Y).DefaultIfEmpty(0).Max();

    public void Update(double dt)
    {
        var headIndex = Array.FindIndex(Runners, r => !r.KO);
        if (headIndex == -1) return;

        var head = Runners.First(r => !r.KO);
        head.S += P.RunSpeed * dt;
        head.Pos = CurrentPath.GetPointAtDistance(head.S);

        int aliveIndex = 0;
        foreach (var r in Runners)
        {
            if (r.KO) continue;
            var targetS = head.S - aliveIndex * P.Spacing;
            r.S = Math.Max(r.S, targetS);
            r.Pos = CurrentPath.GetPointAtDistance(r.S);
            aliveIndex++;
        }
    }
}

public sealed class Drone
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
