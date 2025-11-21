using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PursuitSim_v2.ConsoleApp.Engine
{
    public sealed class UrbanSiegeRunResult
    {
        public int RunIndex { get; set; }
        public bool AttackersWin { get; set; }
        public int AliveAttackers { get; set; }
        public int AliveDefenders { get; set; }
        public string ResultText { get; set; } = "";
    }

}
