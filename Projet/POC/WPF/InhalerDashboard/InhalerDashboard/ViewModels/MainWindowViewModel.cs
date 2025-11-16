using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InhalerDashboard.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;


namespace InhalerDashboard.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IInhalerService _service;
    private readonly CancellationTokenSource _appCts = new();

    public ObservableCollection<InhalerReading> Readings { get; } = new();

    public ObservableCollection<double> FlowPoints { get; } = new();
    public ISeries[] FlowSeries { get; }

    public ObservableCollection<InhalationSummary> Inhalations { get; } = new();

    public ObservableCollection<DeviceEvent> Events { get; } = new();

    [ObservableProperty] private string _connectionStatus = "Disconnected";
    [ObservableProperty] private double _batteryLevel;
    [ObservableProperty] private int _dosesRemaining;
    [ObservableProperty] private string _statusMessage = "Prêt.";
    [ObservableProperty] private string _footer = "© InhalerDashboard - Demo WPF .NET";
    [ObservableProperty] private Color _connectionColor = Colors.Gray;

    [ObservableProperty] private double _averageDurationSeconds;
    [ObservableProperty] private double _averageVolumeMl;
    [ObservableProperty] private double _totalVolumeMl;
    [ObservableProperty] private double _averageIntervalSeconds;

    public MainWindowViewModel(IInhalerService service)
    {
        _service = service;

        FlowSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = FlowPoints,
                GeometrySize = 4,
                Fill = null
            }
        };

        _service.OnReading += OnReading;
        _service.OnInhalationCompleted += OnInhalationCompleted;
        _service.OnDeviceEvent += OnDeviceEvent;
    }

    private void OnReading(object? sender, InhalerReading reading)
    {

        App.Current.Dispatcher.Invoke(() =>
        {
            Readings.Insert(0, reading);
            while (Readings.Count > 500) Readings.RemoveAt(Readings.Count - 1);

            FlowPoints.Add(reading.FlowRate);
            while (FlowPoints.Count > 300) FlowPoints.RemoveAt(0);

            BatteryLevel = reading.BatteryLevel;
            DosesRemaining = reading.DoseCount;
            ConnectionStatus = reading.ConnectionStatus.ToString();

            ConnectionColor = reading.ConnectionStatus switch
            {
                DeviceConnectionStatus.Connected => Colors.LimeGreen,
                DeviceConnectionStatus.Connecting => Colors.Goldenrod,
                _ => Colors.Gray
            };
        });
    }

    private void OnInhalationCompleted(object? sender, InhalationSummary summary)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            Inhalations.Add(summary);
            while (Inhalations.Count > 200) Inhalations.RemoveAt(0);

            RecomputeStats();
        });
    }

    private void OnDeviceEvent(object? sender, DeviceEvent evt)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            Events.Insert(0, evt);
            while (Events.Count > 200) Events.RemoveAt(Events.Count - 1);

            StatusMessage = evt.Message;

        });
    }

    private void RecomputeStats()
    {
        if (Inhalations.Count == 0)
        {
            AverageDurationSeconds = 0;
            AverageVolumeMl = 0;
            TotalVolumeMl = 0;
            AverageIntervalSeconds = 0;
            return;
        }

        TotalVolumeMl = Inhalations.Sum(i => i.VolumeMl);
        AverageVolumeMl = Inhalations.Average(i => i.VolumeMl);
        AverageDurationSeconds = Inhalations.Average(i => i.DurationSeconds);

        if (Inhalations.Count >= 2)
        {
            var ordered = Inhalations.OrderBy(i => i.Start).ToList();
            var intervals = new double[ordered.Count - 1];

            for (int i = 1; i < ordered.Count; i++)
            {
                intervals[i - 1] = (ordered[i].Start - ordered[i - 1].Start).TotalSeconds;
            }

            AverageIntervalSeconds = intervals.Average();
        }
        else
        {
            AverageIntervalSeconds = 0;
        }
    }

    // ===== Commands =====

    [RelayCommand]
    private async Task StartAsync()
    {
        StatusMessage = "Connexion à l'inhalateur…";

        try
        {
            await _service.StartAsync(_appCts.Token);

            // Synchronisation immédiate avec le service
            BatteryLevel = _service.BatteryLevel;
            DosesRemaining = _service.DosesRemaining;
            ConnectionStatus = _service.Status.ToString();

            StatusMessage = "Connecté.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Erreur de démarrage: " + ex.Message;
        }
    }


    [RelayCommand]
    private void Stop()
    {
        _service.Stop();
        StatusMessage = "Arrêté.";
    }

    [RelayCommand]
    private void Inhale()
    {
        _service.TriggerInhalation();
        StatusMessage = "Inhalation simulée";
    }

    [RelayCommand]
    private void Reset()
    {
        _service.Reset();

        Readings.Clear();
        FlowPoints.Clear();
        Inhalations.Clear();
        Events.Clear();

        ConnectionColor = Colors.Gray;
        ConnectionStatus = "Disconnected";
        BatteryLevel = 100;
        DosesRemaining = 200;

        AverageDurationSeconds = 0;
        AverageIntervalSeconds = 0;
        AverageVolumeMl = 0;
        TotalVolumeMl = 0;

        StatusMessage = "Réinitialisé.";
    }
}
