using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PursuitSim_v2.Core.UrbanSiege
{
    public class BattleTeam
    {
        public string Name { get; set; } = "";
        public List<Soldier> Soldiers { get; set; } = new();
        public bool IsDefender { get; set; } = false;

        public int AliveCount => Soldiers.Count(s => !s.IsKO);
        public bool AllKO => AliveCount == 0;
    }
}
