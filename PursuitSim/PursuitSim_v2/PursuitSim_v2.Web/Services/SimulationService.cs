using PursuitSim_v2.Core.Common;
using PursuitSim_v2.Core.DroneSim;

namespace PursuitSim_v2.Blazor.Services;

public sealed class SimulationService
{
    public bool IsRunning { get; private set; }

    public event Action<string>? OnLog;
    public event Action<SimulationSnapshot>? OnTick;
    public async Task RunDroneSimAsync(Scenario scenario)
    {
        if (IsRunning)
            return;

        IsRunning = true;
        OnLog?.Invoke("Simulation started");

        var sim = new Simulation(scenario, "ui");

        sim.LogMessage += msg => OnLog?.Invoke(msg);
        sim.OnTick += snap => OnTick?.Invoke(snap);
        await Task.Run(sim.Run);

        OnLog?.Invoke("Simulation finished");
        IsRunning = false;
    }
}
