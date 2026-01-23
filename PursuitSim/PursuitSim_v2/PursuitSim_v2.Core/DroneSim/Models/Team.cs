using PursuitSim_v2.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PursuitSim_v2.Core.DroneSim.Models
{
    public class Team
    {
        public TeamParams P;
        public TeamState State = TeamState.Stealth;
        public Runner[] Runners;
        public PolyPath CurrentPath;
        public PolyPath? TargetAltPath;
        public double TargetAccessY = -1;
        public double LastCheckpointY = 0;
        public string Name = string.Empty;
        public int Count = 0;
        public double StartX = 300;
        public double StartY = 110;
        public bool IsDefender = false;

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

}
