using InhalerDashboard.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

public interface IInhalerService
{
    event EventHandler<InhalerReading>? OnReading;
    event EventHandler<DeviceEvent>? OnDeviceEvent;
    event EventHandler<InhalationSummary>? OnInhalationCompleted;

    DeviceConnectionStatus Status { get; }
    int DosesRemaining { get; }
    double BatteryLevel { get; }
    PatientProfile Patient { get; }

    Task StartAsync(CancellationToken token);
    void Stop();
    void TriggerInhalation();
    void Reset();
}
