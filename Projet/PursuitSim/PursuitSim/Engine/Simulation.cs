
using PursuitSim.Core;
using PursuitSim.Model;

namespace PursuitSim.Engine;

public sealed class Simulation
{
    readonly Scenario S;
    readonly Team Team;
    readonly Drone Drone;
    readonly double dt = 0.25; // seconds
    double t = 0;

    public Simulation(Scenario s)
    {
        S = s;
        Team = new Team(s.Team, s.MainPath);
        Drone = new Drone(s.Drone);
        Drone.ResetForPatrol();
        LogHeader();
    }

    void LogHeader()
    {
        Console.WriteLine("t(s)\tState\tAlive\tHeadY\tDroneState\tDronePos\tEvent");
    }

    void Log(string evt = "")
    {
        Console.WriteLine($"{t,4:F0}\t{Team.State,-10}\t{Team.AliveCount}\t{Team.HeadY,6:F0}\t{Drone.State,-9}\t{Drone.Pos}\t{evt}");
    }

    public void Run()
    {
        int lastLog = -1;
        while (t < 1200 && Team.State != TeamState.Win && Team.State != TeamState.Fail)
        {
            Step();
            if ((int)Math.Floor(t) != lastLog)
            {
                Log();
                lastLog = (int)Math.Floor(t);
            }
        }
        Log(Team.State == TeamState.Win ? "WIN" : "END");
    }

    void Step()
    {
        t += dt;

        if (Team.HeadY >= S.MapSizeMeters)
        {
            Team.State = TeamState.Win;
            return;
        }

        TeamAI();
        DroneAI();
    }

    void TeamAI()
    {
        bool detected = DroneDetectsAnyRunner();
        if (detected && Team.State == TeamState.Stealth)
        {
            ChooseAltPath();
            Team.State = TeamState.SwitchPath;
        }

        Team.Update(dt);

        if (Team.State == TeamState.SwitchPath && Team.TargetAltPath != null)
        {
            var head = Team.Runners.First(r => !r.KO);
            var accessY = Team.TargetAccessY;
            if (Math.Abs(head.Pos.Y - accessY) <= 2.0 || head.Pos.Y > accessY)
            {
                Team.CurrentPath = Team.TargetAltPath;
                head.S = 0;
                head.Pos = Team.CurrentPath.GetPointAtDistance(0);
                int aliveIndex = 0;
                foreach (var r in Team.Runners.Where(r => !r.KO))
                {
                    r.S = Math.Max(0, head.S - aliveIndex * Team.P.Spacing);
                    r.Pos = Team.CurrentPath.GetPointAtDistance(r.S);
                    aliveIndex++;
                }
                Team.State = TeamState.Evasion;
            }
        }

        Team.LastCheckpointY = Math.Floor(Team.HeadY / 100.0) * 100.0;
    }

    bool DroneDetectsAnyRunner()
    {
        if (Drone.State == DroneState.Destroyed || Drone.State == DroneState.Cooldown) return false;

        foreach (var r in Team.Runners)
        {
            if (r.KO) continue;
            var d = Vec2.Distance(Drone.Pos, r.Pos);
            if (d <= S.Drone.DetectionRange && Geometry.HasLineOfSight(Drone.Pos, r.Pos, S.Obstacles))
            {
                return true;
            }
        }
        return false;
    }

    void ChooseAltPath()
    {
        var headY = Team.HeadY;
        var candidates = S.AccessYs.Where(y => y >= headY + 1).OrderBy(y => y).ToList();
        if (candidates.Count == 0)
        {
            Team.TargetAltPath = S.AltPathA;
            Team.TargetAccessY = headY + 20;
            return;
        }
        var access = candidates.First();
        Team.TargetAccessY = access;
        Team.TargetAltPath = (headY < S.MapSizeMeters * 0.5) ? S.AltPathA : S.AltPathB;
    }

    void DroneAI()
    {
        switch (Drone.State)
        {
            case DroneState.Patrol: PatrolStep(); break;
            case DroneState.Track: TrackStep(); break;
            case DroneState.Attack: AttackStep(); break;
            case DroneState.Destroyed: DestroyedStep(); break;
            case DroneState.Cooldown: CooldownStep(); break;
        }
    }

    void PatrolStep()
    {
        int lanes = 10;
        double laneY = 50 + 100 * Drone.PatrolLane;
        var target = (Drone.Vel.X >= 0) ? new Vec2(1000, laneY) : new Vec2(0, laneY);
        MoveTowards(target, S.Drone.PatrolSpeed);

        if (Vec2.Distance(Drone.Pos, target) < 2)
        {
            Drone.Vel = new Vec2(-Drone.Vel.X, 0);
            if (Math.Abs(Drone.Vel.X) < 1e-9) Drone.Vel = new Vec2(S.Drone.PatrolSpeed, 0);
            if (Drone.Vel.X > 0) Drone.PatrolLane = (Drone.PatrolLane + 1) % lanes;
        }

        var targetRunner = AcquireTarget();
        if (targetRunner != null)
        {
            Drone.Target = targetRunner;
            Drone.ReactTimer = S.Drone.ReactLatency;
            Drone.State = DroneState.Track;
            Log("DETECTED");
        }
    }

    void TrackStep()
    {
        if (Drone.Target == null || Drone.Target.KO)
        {
            Drone.State = DroneState.Patrol;
            return;
        }

        if (Drone.ReactTimer > 0)
        {
            Drone.ReactTimer -= dt;
            return;
        }

        bool los = Geometry.HasLineOfSight(Drone.Pos, Drone.Target.Pos, S.Obstacles);
        if (!los) Drone.TimeSinceLoS += dt; else Drone.TimeSinceLoS = 0;
        if (Drone.TimeSinceLoS >= S.Drone.LoseSightAfter)
        {
            Drone.State = DroneState.Patrol;
            Drone.Target = null;
            return;
        }

        MoveTowards(Drone.Target.Pos, S.Drone.ChaseSpeed);

        if (Vec2.Distance(Drone.Pos, Drone.Target.Pos) <= S.Drone.AttackRadius)
        {
            Drone.State = DroneState.Attack;
        }
    }

    void AttackStep()
    {
        int kos = 0;
        foreach (var r in Team.Runners)
        {
            if (r.KO) continue;
            if (Vec2.Distance(Drone.Pos, r.Pos) <= S.Drone.AttackRadius)
            {
                r.KO = true;
                kos++;
                if (kos == 2) break;
            }
        }

        Log(kos >= 2 ? "ATTACK: DOUBLE KO" : (kos == 1 ? "ATTACK: SINGLE KO" : "ATTACK: MISS"));

        if (kos >= 2)
        {
            Team.State = TeamState.Fail; // double KO -> retraite -> échec
        }
        else if (kos == 1)
        {
            Team.State = TeamState.ContinueWith2;
            if (Team.AliveCount == 0) Team.State = TeamState.Fail;
        }

        Drone.State = DroneState.Destroyed;
        Drone.Cooldown = S.Drone.RespawnSeconds;
    }

    void DestroyedStep()
    {
        Drone.State = DroneState.Cooldown;
    }

    void CooldownStep()
    {
        Drone.Cooldown -= dt;
        if (Drone.Cooldown <= 0)
        {
            Drone.ResetForPatrol();
            Log("DRONE RESPAWN");
        }
    }

    Runner? AcquireTarget()
    {
        Runner? best = null;
        double bestD = double.MaxValue;
        foreach (var r in Team.Runners)
        {
            if (r.KO) continue;
            double d = Vec2.Distance(Drone.Pos, r.Pos);
            if (d <= S.Drone.DetectionRange && Geometry.HasLineOfSight(Drone.Pos, r.Pos, S.Obstacles))
            {
                if (d < bestD) { bestD = d; best = r; }
            }
        }
        return best;
    }

    void MoveTowards(Vec2 target, double speed)
    {
        var dir = (target - Drone.Pos).Normalized();
        Drone.Pos += dir * speed * dt;
    }
}
