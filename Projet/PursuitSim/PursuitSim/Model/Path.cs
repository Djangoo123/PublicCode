
using PursuitSim.Core;

namespace PursuitSim.Model;

public sealed class PolyPath
{
    public readonly List<Vec2> Points = new();
    public PolyPath(IEnumerable<Vec2> pts) => Points.AddRange(pts);

    public Vec2 GetPointAtDistance(double s)
    {
        if (Points.Count == 0) return new Vec2(0,0);
        if (Points.Count == 1) return Points[0];
        double remaining = s;
        for (int i = 0; i < Points.Count - 1; i++)
        {
            var a = Points[i];
            var b = Points[i + 1];
            var seg = Vec2.Distance(a, b);
            if (remaining <= seg) return a + (b - a) * (remaining / seg);
            remaining -= seg;
        }
        return Points[^1];
    }

    public double TotalLength()
    {
        double sum = 0;
        for (int i = 0; i < Points.Count - 1; i++)
            sum += Vec2.Distance(Points[i], Points[i + 1]);
        return sum;
    }
}
