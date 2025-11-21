using PursuitSim_v2.Core;

namespace PursuitSim_v2.Core.Common;

public enum TeamState { Stealth, SwitchPath, Evasion, ContinueWith2, Retreat, Win, Fail }
public enum DroneState { Patrol, Track, Attack, Bombing, Destroyed, Cooldown }
public enum DroneType { Hunter, Bomber }
public sealed class Runner
{
    public bool KO;
    public double S; // distance along current path
    public Vec2 Pos;
}
