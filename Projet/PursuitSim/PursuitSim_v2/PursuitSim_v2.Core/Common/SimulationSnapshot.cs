namespace PursuitSim_v2.Core.Common
{
    public record SimulationSnapshot(
        double Time,
        Position Drone,
        Position Target,
        double Distance
    );
}
