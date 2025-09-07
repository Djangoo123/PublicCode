using System;
using System.Collections.Generic;
using System.Linq;
using PursuitSim.Core;

namespace PursuitSim.Model;

public sealed class Scenario
{
    public string Name { get; set; } = string.Empty;
    public double MapSizeMeters { get; set; } = 1000.0;

    public PolyPath MainPath { get; set; } = new PolyPath(new[] { new Vec2(500, 0), new Vec2(500, 1000) });
    public PolyPath AltPathA { get; set; } = new PolyPath(new[] { new Vec2(300, 250), new Vec2(300, 700) });
    public PolyPath AltPathB { get; set; } = new PolyPath(new[] { new Vec2(700, 400), new Vec2(700, 850) });
    public List<double> AccessYs { get; set; } = new();
    public List<Rect> Obstacles { get; set; } = new();

    public DroneParams Drone { get; set; } = new();
    public TeamParams Team { get; set; } = new();
}

public sealed class DroneParams
{
    public double PatrolSpeed = 16.7;     // m/s ~ 60 km/h
    public double ChaseSpeed = 16.7;      // m/s (55.6 for "hard mode")
    public double DetectionRange = 200.0; // m
    public double AttackRadius = 3.0;     // m
    public double ReactLatency = 0.5;     // s
    public double LoseSightAfter = 3.0;   // s sans LoS
    public double RespawnSeconds = 300.0; // 5 min
    public double FovCosine = Math.Cos(Math.PI / 2.0); // 90° de FOV
}

public sealed class TeamParams
{
    public int Count = 3;
    public double RunSpeed = 4.17; // m/s ~ 15 km/h
    public double Spacing = 6.0;   // m between members
}

public static class Scenarios
{
    public static Scenario PlainWithHedges()
    {
        var s = new Scenario { Name = "Plain + Hedges" };

        for (int y = 150; y <= 900; y += 150)
            s.Obstacles.Add(new Rect(0, y - 1, 1000, 2));

        s.AccessYs = Enumerable.Range(1, 9).Select(i => i * 100.0).ToList();

        s.MainPath = new PolyPath(new[] { new Vec2(500, 0), new Vec2(500, 1000) });
        s.AltPathA = new PolyPath(new[] { new Vec2(500, 250), new Vec2(300, 250), new Vec2(300, 700), new Vec2(500, 700) });
        s.AltPathB = new PolyPath(new[] { new Vec2(500, 400), new Vec2(700, 400), new Vec2(700, 850), new Vec2(500, 850) });

        return s;
    }

    public static Scenario UrbanGrid()
    {
        var s = new Scenario { Name = "Urban Grid" };

        for (int x = 0; x < 1000; x += 60)
            for (int y = 0; y < 1000; y += 60)
                s.Obstacles.Add(new Rect(x + 5, y + 5, 50, 50));

        s.AccessYs = Enumerable.Range(1, 9).Select(i => i * 100.0).ToList();

        s.MainPath = new PolyPath(new[] { new Vec2(500, 0), new Vec2(500, 1000) });
        s.AltPathA = new PolyPath(new[] { new Vec2(500, 300), new Vec2(420, 300), new Vec2(420, 700), new Vec2(500, 700) });
        s.AltPathB = new PolyPath(new[] { new Vec2(500, 450), new Vec2(580, 450), new Vec2(580, 850), new Vec2(500, 850) });

        return s;
    }

    public static Scenario MixedClearingFinal()
    {
        var s = new Scenario { Name = "Mixed: Cover then Clearing" };

        for (int x = 0; x < 1000; x += 80)
            for (int y = 0; y < 600; y += 80)
                s.Obstacles.Add(new Rect(x + 10, y + 10, 50, 50));

        s.Obstacles.Add(new Rect(450, 620, 100, 20));
        s.Obstacles.Add(new Rect(300, 720, 120, 20));

        s.AccessYs = new List<double> { 200, 300, 400, 500, 650, 700, 800, 900 };

        s.MainPath = new PolyPath(new[] { new Vec2(500, 0), new Vec2(500, 1000) });
        s.AltPathA = new PolyPath(new[] { new Vec2(500, 250), new Vec2(320, 250), new Vec2(320, 700), new Vec2(500, 700) });
        s.AltPathB = new PolyPath(new[] { new Vec2(500, 400), new Vec2(680, 400), new Vec2(680, 850), new Vec2(500, 850) });

        return s;
    }
}
