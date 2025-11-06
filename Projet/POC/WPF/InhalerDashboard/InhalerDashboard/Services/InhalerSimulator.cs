using System;
using System.Threading;
using System.Threading.Tasks;
using InhalerDashboard.Models;

namespace InhalerDashboard.Services;

public interface IInhalerService
{
    event EventHandler<InhalerReading>? OnReading;
    DeviceConnectionStatus Status { get; }
    int DosesRemaining { get; }
    double BatteryLevel { get; }
    Task StartAsync(CancellationToken token);
    void Stop();
    void TriggerInhalation();
    void Reset();
}

public sealed class InhalerSimulator : IInhalerService
{
    private CancellationTokenSource? _cts;
    private readonly Random _rng = new();
    private Task? _loop;
    private readonly object _gate = new();

    public event EventHandler<InhalerReading>? OnReading;
    public DeviceConnectionStatus Status { get; private set; } = DeviceConnectionStatus.Disconnected;
    public int DosesRemaining { get; private set; } = 200;
    public double BatteryLevel { get; private set; } = 100.0;

    public async Task StartAsync(CancellationToken token)
    {
        if (Status == DeviceConnectionStatus.Connected) return;

        Status = DeviceConnectionStatus.Connecting;
        Publish(0);

        _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        var ct = _cts.Token;

        // fake connect delay
        await Task.Delay(700, ct);
        Status = DeviceConnectionStatus.Connected;
        Publish(0);

        _loop = Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                // Small idle drift + slow battery drain
                var drift = _rng.NextDouble() * 2.0;
                BatteryLevel = Math.Max(0, BatteryLevel - 0.01);
                Publish(drift);
                await Task.Delay(500, ct);
            }
        }, ct);
    }

    public void Stop()
    {
        if (Status == DeviceConnectionStatus.Disconnected) return;
        _cts?.Cancel();
        Status = DeviceConnectionStatus.Disconnected;
        Publish(0);
    }

    public void Reset()
    {
        Stop();
        DosesRemaining = 200;
        BatteryLevel = 100.0;
        Status = DeviceConnectionStatus.Disconnected;
        Publish(0);
    }

    public void TriggerInhalation()
    {
        if (Status != DeviceConnectionStatus.Connected) return;
        if (DosesRemaining <= 0) return;

        lock (_gate)
        {
            // Simulate a short burst: peak flow ~ 60-90 L/min
            var peak = 60 + _rng.NextDouble() * 30;
            for (int i = 0; i < 8; i++)
            {
                var t = i / 7.0;
                var flow = peak * Math.Sin(Math.PI * t); 
                Publish(flow);
            }

            DosesRemaining--;
            BatteryLevel = Math.Max(0, BatteryLevel - 0.2);
            Publish(0);
        }
    }

    private void Publish(double flow)
    {
        OnReading?.Invoke(this, new InhalerReading
        {
            Timestamp = DateTime.Now,
            FlowRate = Math.Round(flow, 2),
            DoseCount = DosesRemaining,
            BatteryLevel = Math.Round(BatteryLevel, 1),
            ConnectionStatus = Status
        });
    }
}