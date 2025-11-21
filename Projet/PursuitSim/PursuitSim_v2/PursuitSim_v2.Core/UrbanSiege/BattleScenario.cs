using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PursuitSim_v2.Core.UrbanSiege
{
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
