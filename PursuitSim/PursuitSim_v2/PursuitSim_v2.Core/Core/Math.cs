
namespace PursuitSim_v2.Core;

public readonly struct Vec2
{
    public readonly double X;
    public readonly double Y;
    public Vec2(double x, double y) { X = x; Y = y; }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vec2 operator *(Vec2 a, double k) => new(a.X * k, a.Y * k);
    public static Vec2 operator /(Vec2 a, double k) => new(a.X / k, a.Y / k);

    public double Length() => Math.Sqrt(X * X + Y * Y);
    public double LengthSq() => X * X + Y * Y;
    public Vec2 Normalized()
    {
        var len = Length();
        return len > 1e-9 ? this / len : new Vec2(0, 0);
    }

    public static double Dot(in Vec2 a, in Vec2 b) => a.X * b.X + a.Y * b.Y;
    public static double Distance(in Vec2 a, in Vec2 b) => (a - b).Length();
    public static double DistanceSq(in Vec2 a, in Vec2 b) => (a - b).LengthSq();

    public override string ToString() => $"({X:F1},{Y:F1})";
}
