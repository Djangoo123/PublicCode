using System;
using System.Threading;
using System.Threading.Tasks;
using InhalerDashboard.Models;

namespace InhalerDashboard.Services;

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

    private int _currentInhalationId = 0;

    public event EventHandler<DeviceEvent>? OnDeviceEvent;
    public event EventHandler<InhalationSummary>? OnInhalationCompleted;

    public InhalerSimulator()
    {
        Patient = new PatientProfile
        {
            Age = 45,
            LungCapacityLiters = 4.5,
            Pathology = Pathology.Asthma
        };
    }
    public PatientProfile Patient { get; }
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

        //_loop = Task.Run(async () =>
        //{
        //    while (!ct.IsCancellationRequested)
        //    {
        //        // Small idle drift + slow battery drain
        //        var drift = _rng.NextDouble() * 2.0;
        //        BatteryLevel = Math.Max(0, BatteryLevel - 0.01);
        //        Publish(drift);
        //        await Task.Delay(500, ct);
        //    }
        //}, ct);
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

    private void Publish(double flow, int? inhalationId = null)
    {
        OnReading?.Invoke(this, new InhalerReading
        {
            Timestamp = DateTime.Now,
            FlowRate = Math.Round(flow, 2),
            DoseCount = DosesRemaining,
            BatteryLevel = Math.Round(BatteryLevel, 1),
            ConnectionStatus = Status,
            InhalationId = inhalationId
        });
    }

    public void TriggerInhalation()
    {
        if (Status != DeviceConnectionStatus.Connected) return;
        if (DosesRemaining <= 0) return;

        lock (_gate)
        {
            _currentInhalationId++;
        }
        int inhalationId = _currentInhalationId;

        DateTime start = DateTime.Now;

        int steps = 30;
        double dtSeconds = 1.5 / steps;

        double basePeak = 80.0 * Patient.PeakFlowFactor;
        double noise = _rng.NextDouble() * 10 - 5;
        double peak = Math.Max(20, basePeak + noise);

        bool airLeak = _rng.NextDouble() < 0.1;
        bool sensorFailure = _rng.NextDouble() < 0.03;

        if (airLeak)
        {
            peak *= 0.3;
            OnDeviceEvent?.Invoke(this, new DeviceEvent
            {
                Timestamp = DateTime.Now,
                Type = DeviceEventType.AirLeak,
                Message = "Suspicion de fuite d’air durant l’inhalation."
            });
        }

        if (sensorFailure)
        {
            OnDeviceEvent?.Invoke(this, new DeviceEvent
            {
                Timestamp = DateTime.Now,
                Type = DeviceEventType.SensorFailure,
                Message = "Erreur capteur : valeurs invalides."
            });
        }

        double volumeLiters = 0.0;

        for (int i = 0; i < steps; i++)
        {
            double t = i / (double)(steps - 1);
            double flow = peak * Math.Sin(Math.PI * t);

            if (sensorFailure)
            {
                flow = 0;
            }

            volumeLiters += flow * dtSeconds / 60.0;

            Publish(flow, inhalationId);

            Task.Delay((int)(dtSeconds * 1000)); 
        }

        DateTime end = DateTime.Now;

        DosesRemaining--;
        BatteryLevel = Math.Max(0, BatteryLevel - 0.2);

        if (BatteryLevel < 10)
        {
            OnDeviceEvent?.Invoke(this, new DeviceEvent
            {
                Timestamp = DateTime.Now,
                Type = DeviceEventType.LowBattery,
                Message = "Batterie critique (< 10%)."
            });
        }

        double volumeMl = Math.Round(volumeLiters * 1000.0, 1);

        OnInhalationCompleted?.Invoke(this, new InhalationSummary
        {
            Start = start,
            End = end,
            VolumeMl = volumeMl
        });

        Publish(0, inhalationId);
    }

}