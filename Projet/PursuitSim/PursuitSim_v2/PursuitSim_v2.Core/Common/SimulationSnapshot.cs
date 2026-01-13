namespace PursuitSim_v2.Core.Common
{
    public record SimulationSnapshot(
        double Time,
        Position Drone,
        Position? Target,
        double Distance,
        IReadOnlyList<RunnerSnapshot> Runners
    );

    public record RunnerSnapshot(
        double X,
        double Y,
        bool KO
    );

}
