using PursuitSim_v2.Core.DroneSim;

namespace PursuitSim_v2.Blazor.Services;

public sealed class SimulationService
{
    public bool IsRunning { get; private set; }

    public event Action<string>? OnLog;

    public async Task RunDroneSimAsync(Scenario scenario)
    {
        if (IsRunning)
            return;

        IsRunning = true;
        OnLog?.Invoke("Simulation started");

        var sim = new Simulation(scenario, "ui");

        sim.LogMessage += msg => OnLog?.Invoke(msg);

        await Task.Run(sim.Run);

        OnLog?.Invoke("Simulation finished");
        IsRunning = false;
    }
}
