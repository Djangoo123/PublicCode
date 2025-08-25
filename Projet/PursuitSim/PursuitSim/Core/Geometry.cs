
namespace PursuitSim.Core;

public readonly struct Rect
{
    public readonly double X, Y, W, H;
    public Rect(double x, double y, double w, double h) { X = x; Y = y; W = w; H = h; }
    public double X2 => X + W;
    public double Y2 => Y + H;
    public bool Contains(Vec2 p) => p.X >= X && p.X <= X2 && p.Y >= Y && p.Y <= Y2;
}

public static class Geometry
{
    public static bool LineSegmentsIntersect(Vec2 p, Vec2 p2, Vec2 q, Vec2 q2)
    {
        double o1 = Orientation(p, p2, q);
        double o2 = Orientation(p, p2, q2);
        double o3 = Orientation(q, q2, p);
        double o4 = Orientation(q, q2, p2);

        if (o1 * o2 < 0 && o3 * o4 < 0) return true;

        if (Math.Abs(o1) < 1e-9 && OnSegment(p, q, p2)) return true;
        if (Math.Abs(o2) < 1e-9 && OnSegment(p, q2, p2)) return true;
        if (Math.Abs(o3) < 1e-9 && OnSegment(q, p, q2)) return true;
        if (Math.Abs(o4) < 1e-9 && OnSegment(q, p2, q2)) return true;

        return false;
    }

    static double Orientation(Vec2 a, Vec2 b, Vec2 c)
        => (b.Y - a.Y) * (c.X - b.X) - (b.X - a.X) * (c.Y - b.Y);

    static bool OnSegment(Vec2 a, Vec2 b, Vec2 c)
        => Math.Min(a.X, c.X) - 1e-9 <= b.X && b.X <= Math.Max(a.X, c.X) + 1e-9 &&
           Math.Min(a.Y, c.Y) - 1e-9 <= b.Y && b.Y <= Math.Max(a.Y, c.Y) + 1e-9;

    public static bool SegmentIntersectsRect(Vec2 a, Vec2 b, Rect r)
    {
        if (r.Contains(a) || r.Contains(b)) return true;
        var r1 = new Vec2(r.X, r.Y);
        var r2 = new Vec2(r.X2, r.Y);
        var r3 = new Vec2(r.X2, r.Y2);
        var r4 = new Vec2(r.X, r.Y2);
        return LineSegmentsIntersect(a, b, r1, r2) ||
               LineSegmentsIntersect(a, b, r2, r3) ||
               LineSegmentsIntersect(a, b, r3, r4) ||
               LineSegmentsIntersect(a, b, r4, r1);
    }

    public static bool HasLineOfSight(Vec2 a, Vec2 b, IReadOnlyList<Rect> obstacles)
    {
        foreach (var r in obstacles)
            if (SegmentIntersectsRect(a, b, r)) return false;
        return true;
    }
}
