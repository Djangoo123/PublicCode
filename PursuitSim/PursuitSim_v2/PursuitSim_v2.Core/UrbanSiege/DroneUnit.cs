using PursuitSim_v2.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PursuitSim_v2.Core.UrbanSiege
{
    public class DroneUnit
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Vec2 Pos { get; set; } = new(0, 0);
        public DroneType Type { get; set; } = DroneType.Hunter;
        public double Cooldown { get; set; } = 0;
        public bool Active => Cooldown <= 0;
    }
}
