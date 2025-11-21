using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PursuitSim_v2.Core.Common
{
    public readonly struct Rect
    {
        public readonly double X, Y, W, H;

        public Rect(double x, double y, double w, double h)
        {
            X = x; Y = y; W = w; H = h;
        }

        public double X2 => X + W;
        public double Y2 => Y + H;

        public bool Contains(Vec2 p) =>
            p.X >= X && p.X <= X2 &&
            p.Y >= Y && p.Y <= Y2;
    }
}
