using System;
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

    readonly string _scenarioId;
    readonly string _outDir;
    CsvExporter? _csv;
    int _lastLoggedSecond = -1;

    readonly Random rng = new Random();
    double sprintTimer = 0;
    public Simulation(Scenario s, string scenarioId, string outDir = "out")
    {
        S = s;
        _scenarioId = scenarioId;
        _outDir = string.IsNullOrWhiteSpace(outDir) ? "out" : outDir;

        Team = new Team(s.Team, s.MainPath);
        Drone = new Drone(s.Drone);
        Drone.ResetForPatrol();

        _csv = new CsvExporter(_outDir, S.Name, _scenarioId, DateTime.Now);

        LogHeader();
    }

    void LogHeader()
    {
        Console.WriteLine("t(s)\tState\tAlive\tHeadY\tDroneState\tDronePos\tEvent");
    }

    void Log(string evt = "")
    {
        Console.WriteLine($"{t,4:F0}\t{Team.State,-10}\t{Team.AliveCount}\t{Team.HeadY,6:F0}\t{Drone.State,-9}\t{Drone.Pos}\t{evt}");

        if (!string.IsNullOrWhiteSpace(evt) && _csv is not null)
        {
            string type = evt;
            string? note = null;
            var idx = evt.IndexOf(':');
            if (idx > 0)
            {
                type = evt[..idx].Trim();
                note = evt[(idx + 1)..].Trim();
            }
            _csv.WriteEvent(t, S, Team, Drone, type, note);
        }
    }

    public void Run()
    {
        while (t < 1200 && Team.State != TeamState.Win && Team.State != TeamState.Fail)
        {
            Step();

            var sec = (int)Math.Floor(t);
            if (sec != _lastLoggedSecond)
            {
                _csv?.WriteTick(t, S, Team, Drone);
                Log();
                _lastLoggedSecond = sec;
            }
        }

        Log(Team.State == TeamState.Win ? "WIN" : "END");
        _csv?.WriteEvent(t, S, Team, Drone, Team.State == TeamState.Win ? "WIN" : "END");
        _csv?.Close();
        _csv = null;
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

        // --- Team reaction against drone ---
        foreach (var r in Team.Runners.Where(r => !r.KO))
        {
            var dist = Vec2.Distance(Drone.Pos, r.Pos);

            if (dist <= 100 && Team.State == TeamState.Stealth)
            {
                Log("ALERT: Drone spotted (<100m)");
                Team.State = TeamState.Evasion;
            }

            if (dist <= 20)
            {
                bool droneFonse = Drone.State == DroneState.Track || Drone.State == DroneState.Attack;
                if (droneFonse)
                {
                    if (rng.NextDouble() < 0.1)
                    {
                        Log("RETELIATE: Drone down !");
                        Drone.State = DroneState.Destroyed;
                        Drone.Cooldown = S.Drone.RespawnSeconds;
                    }
                    else
                    {
                        Log("RETELIATE: failed");
                    }
                }
                else
                {
                    Team.P.RunSpeed = 5.56; // ~20 km/h
                    Log("Run: sprint stated !");
                }
            }
        }

        if (Team.P.RunSpeed > 4.17)
        {
            if (sprintTimer <= 0)
                sprintTimer = 10.0; // 10 seconds sprint
            else
            {
                sprintTimer -= dt;
                if (sprintTimer <= 0)
                {
                    Team.P.RunSpeed = 4.17; // bac to ~15 km/h
                    Log("Sprint over, back to normal speed");
                }
            }
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

                Log("SWITCH: AltPath");
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
            Log("ALT: fallback access");
            return;
        }
        var access = candidates.First();
        Team.TargetAccessY = access;
        Team.TargetAltPath = (headY < S.MapSizeMeters * 0.5) ? S.AltPathA : S.AltPathB;
        Log($"ALT: target accessY={access:0}");
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
            case DroneState.Bombing: BombingStep(); break;
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
            Drone.Target = targetRunner;
            Drone.ReactTimer = S.Drone.ReactLatency;

            if (Drone.Type == DroneType.Hunter)
                Drone.State = DroneState.Track;
            else
                Drone.State = DroneState.Bombing;
            Log("DETECTED");
        }
    }

    void BombingStep()
    {
        // Follow the head of the team
        var target = new Vec2(Team.Runners.First(r => !r.KO).Pos.X, Team.HeadY);
        Drone.Pos = new Vec2(target.X, target.Y);

        if (Drone.BombTimer <= 0)
        {
            // Drop a bomb
            var tFall = Math.Sqrt(2 * Drone.BombAltitude / 9.81);
            Drone.BombTimer = tFall;
            Log($"BOMB DROPPED from {Drone.BombAltitude}m, impact in {tFall:F1}s");
        }
        else
        {
            Drone.BombTimer -= dt;
            if (Drone.BombTimer <= 0)
            {
                // Impact: 50% chance to eliminate ~75% of the team
                if (rng.NextDouble() < 0.5)
                {
                    int toKO = (int)Math.Ceiling(Team.Runners.Count(r => !r.KO) * 0.75);
                    foreach (var r in Team.Runners.Where(r => !r.KO).Take(toKO))
                        r.KO = true;

                    Log($"EXPLOSION: {toKO} soldiers KO (≈75% of team)");
                }
                else
                {
                    Log("EXPLOSION: minor effect, team survives");
                }

                // Bomber is destroyed after attack
                Drone.State = DroneState.Destroyed;
                Drone.Cooldown = S.Drone.RespawnSeconds;
            }
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
            Log("LOST");
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
            Team.State = TeamState.Fail; 
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
            Log("RESPAWN");
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
