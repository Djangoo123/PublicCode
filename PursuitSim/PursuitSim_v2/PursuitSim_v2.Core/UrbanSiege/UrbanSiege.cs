using PursuitSim_v2.Core;
using PursuitSim_v2.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PursuitSim_v2.Core.UrbanSiege;

public class UrbanSiegeEngine
{
    private readonly BattleScenario scenario;
    private readonly Random rng = new();
    private double t = 0;
    private readonly double dt = 0.2; // time step
    public event Action<string>? Log;

    public UrbanSiegeEngine(BattleScenario s)
    {
        scenario = s;
    }

    public void Run()
    {
        Log?.Invoke($"=== {scenario.Name} Simulation ===");
        while (!IsFinished())
        {
            Step();
            t += dt;
        }

        PrintOutcome();
    }

    private void Step()
    {
        MoveAttackers();
        HandleDrones();
        DefendersFire();
        AttackersFire();
    }

    private void MoveAttackers()
    {
        foreach (var a in scenario.Attackers.Soldiers.Where(s => !s.IsKO))
            a.Pos = new Vec2(a.Pos.X, a.Pos.Y + 5 * dt); // 5 m/s
    }

    private void HandleDrones()
    {
        foreach (var d in scenario.Drones)
        {
            if (!d.Active)
            {
                d.Cooldown -= dt;
                continue;
            }

            var target = scenario.Attackers.Soldiers.Where(s => !s.IsKO)
                .OrderBy(s => Vec2.Distance(s.Pos, d.Pos)).FirstOrDefault();
            if (target == null) continue;

            double dist = Vec2.Distance(d.Pos, target.Pos);
            if (d.Type == DroneType.Hunter && dist < 3)
            {
                target.IsKO = true;
                d.Cooldown = 300; // 5 minutes
                Log?.Invoke($"[{t:F1}s] Drone (Hunter) hit attacker at {target.Pos.Y:F1}m");
            }
            else if (d.Type == DroneType.Bomber && dist < 10 && rng.NextDouble() < 0.1)
            {
                var victims = scenario.Attackers.Soldiers
                    .Where(s => !s.IsKO && Vec2.Distance(s.Pos, d.Pos) < 5)
                    .Take(5).ToList();
                foreach (var v in victims) v.IsKO = true;
                Log?.Invoke($"[{t:F1}s] Drone (Bomber) explosion — {victims.Count} attackers KO");
                d.Cooldown = 300;
            }
        }
    }

    private void DefendersFire()
    {
        foreach (var def in scenario.Defenders)
        {
            foreach (var shooter in def.Soldiers.Where(s => !s.IsKO))
            {
                var targets = scenario.Attackers.Soldiers
                    .Where(a => !a.IsKO && Vec2.Distance(a.Pos, shooter.Pos) <= scenario.AttackRangeDefenders);

                foreach (var targetSoldier in targets)
                {
                    if (rng.NextDouble() < scenario.DefenderHitChance)
                    {
                        targetSoldier.IsKO = true;
                        Log?.Invoke($"[{t:F1}s] Defender {def.Name} killed attacker at {targetSoldier.Pos.Y:F1}m");
                    }
                }
            }
        }
    }


    private void AttackersFire()
    {
        foreach (var a in scenario.Attackers.Soldiers.Where(s => !s.IsKO && s.Pos.Y >= 70))
        {
            var defenders = scenario.Defenders.SelectMany(d => d.Soldiers)
                .Where(s => !s.IsKO && Vec2.Distance(s.Pos, a.Pos) <= scenario.AttackRangeAttackers);
            foreach (var d in defenders)
            {
                if (rng.NextDouble() < scenario.AttackerHitChance)
                {
                    d.IsKO = true;
                    Log?.Invoke($"[{t:F1}s] Attacker hit defender {d.TeamName}");
                }
            }
        }
    }

    private bool IsFinished()
    {
        if (scenario.Attackers.Soldiers.Count(s => !s.IsKO && s.Pos.Y >= 100) >= scenario.VictoryThreshold)
            return true;
        if (scenario.Attackers.Soldiers.All(s => s.IsKO))
            return true;
        return false;
    }

    private void PrintOutcome()
    {
        int aliveAttackers = scenario.Attackers.Soldiers.Count(s => !s.IsKO);
        int aliveDefenders = scenario.Defenders.SelectMany(d => d.Soldiers).Count(s => !s.IsKO);

        Log?.Invoke("");
        Log?.Invoke("=== Battle Summary ===");
        Log?.Invoke($"Attackers alive: {aliveAttackers}");
        Log?.Invoke($"Defenders alive: {aliveDefenders}");

        if (aliveAttackers >= scenario.VictoryThreshold)
            Log?.Invoke(">> RESULT: ATTACKERS WIN");
        else
            Log?.Invoke(">> RESULT: DEFENDERS HOLD");
    }

}