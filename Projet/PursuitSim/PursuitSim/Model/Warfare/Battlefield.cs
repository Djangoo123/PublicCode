using PursuitSim.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PursuitSim.Model.Warfare
{
    public class Soldier
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Vec2 Pos { get; set; } = new(0, 0);
        public bool IsKO { get; set; } = false;
        public bool IsDefender { get; set; } = false;
        public string TeamName { get; set; } = "";
    }

    public class BattleTeam
    {
        public string Name { get; set; } = "";
        public List<Soldier> Soldiers { get; set; } = new();
        public bool IsDefender { get; set; } = false;

        public int AliveCount => Soldiers.Count(s => !s.IsKO);
        public bool AllKO => AliveCount == 0;
    }

    public class DroneUnit
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Vec2 Pos { get; set; } = new(0, 0);
        public DroneType Type { get; set; } = DroneType.Hunter;
        public double Cooldown { get; set; } = 0;
        public bool Active => Cooldown <= 0;
    }

    public class BattleScenario
    {
        public string Name { get; set; } = "Urban Siege";
        public List<BattleTeam> Defenders { get; set; } = new();
        public BattleTeam Attackers { get; set; } = new();
        public List<DroneUnit> Drones { get; set; } = new();

        public double AttackRangeDefenders { get; set; } = 30;
        public double AttackRangeAttackers { get; set; } = 30;
        public double DefenderHitChance { get; set; } = 0.25;
        public double AttackerHitChance { get; set; } = 0.20;
        public int VictoryThreshold { get; set; } = 50;

        public double MapWidth { get; set; } = 1000;
        public double MapHeight { get; set; } = 200;
    }
}
