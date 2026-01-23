using System;
using System.Collections.Generic;
using System.Linq;
using PursuitSim_v2.Core;
using PursuitSim_v2.Core.Common;
using PursuitSim_v2.Core.DroneSim.Models;
using PursuitSim_v2.Core.DroneSim.Models.Mines;

namespace PursuitSim_v2.Core.DroneSim;

public sealed class Scenario
{
    public string Name { get; set; } = string.Empty;
    public double MapSizeMeters { get; set; } = 1000.0;

    public PolyPath MainPath { get; set; } = new PolyPath([new Vec2(500, 0), new Vec2(500, 1000)]);
    public PolyPath AltPathA { get; set; } = new PolyPath([new Vec2(300, 250), new Vec2(300, 700)]);
    public PolyPath AltPathB { get; set; } = new PolyPath([new Vec2(700, 400), new Vec2(700, 850)]);
    public List<double> AccessYs { get; set; } = [];
    public List<Rect> Obstacles { get; set; } = [];

    public DroneParams Drone { get; set; } = new();
    public TeamParams Team { get; set; } = new();
    public List<Mine> Mines { get; set; } = [];
    public List<Team> DefenderTeams { get; set; } = [];
    public Team? AttackerTeam { get; set; }
    public List<Drone> Drones { get; set; } = [];

    public double AttackRangeDefenders { get; set; }
    public double AttackRangeAttackers { get; set; }
    public double DefenderHitChance { get; set; }
    public double AttackerHitChance { get; set; }
    public int VictoryThreshold { get; set; }

}