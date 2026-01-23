using PursuitSim_v2.Core;
using PursuitSim_v2.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PursuitSim_v2.Core.UrbanSiege
{
    public static class UrbanSiegeFactory
    {
        public static BattleScenario Create()
        {
            var s = new BattleScenario
            {
                Name = "Urban Siege",
                MapWidth = 1000,
                MapHeight = 200,
                AttackRangeDefenders = 30,
                AttackRangeAttackers = 30,
                DefenderHitChance = 0.25,
                AttackerHitChance = 0.20,
                VictoryThreshold = 50
            };

            var rng = new Random();

            // === DEFENDER TEAMS ===
            s.Defenders =
            [
                CreateDefenderTeam("Alpha", x: 300, y: 110),
                CreateDefenderTeam("Bravo", x: 500, y: 110),
                CreateDefenderTeam("Charlie", x: 700, y: 110),
            ];

            // === ATTACKER TEAM ===
            var attackers = new BattleTeam
            {
                Name = "Assault",
                IsDefender = false
            };

            for (int i = 0; i < 100; i++)
            {
                attackers.Soldiers.Add(new Soldier
                {
                    TeamName = "Assault",
                    Pos = new Vec2(rng.Next(400, 600), rng.Next(0, 20)),
                    IsDefender = false
                });
            }

            s.Attackers = attackers;

            // === DRONES ===
            s.Drones = new List<DroneUnit>();
            for (int i = 0; i < 10; i++)
            {
                s.Drones.Add(new DroneUnit
                {
                    Pos = new Vec2(rng.Next(200, 800), rng.Next(0, 80)),
                    Type = rng.NextDouble() < 0.5 ? DroneType.Hunter : DroneType.Bomber
                });
            }

            return s;
        }

        private static BattleTeam CreateDefenderTeam(string name, double x, double y)
        {
            var team = new BattleTeam
            {
                Name = name,
                IsDefender = true
            };

            for (int i = 0; i < 6; i++)
            {
                team.Soldiers.Add(new Soldier
                {
                    TeamName = name,
                    IsDefender = true,
                    Pos = new Vec2(x + i * 2, y)
                });
            }

            return team;
        }
    }
}
