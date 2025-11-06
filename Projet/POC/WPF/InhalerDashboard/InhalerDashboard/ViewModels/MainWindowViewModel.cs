using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InhalerDashboard.Models;
using InhalerDashboard.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;

namespace InhalerDashboard.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IInhalerService _service;
    private readonly CancellationTokenSource _appCts = new();

    public ObservableCollection<InhalerReading> Readings { get; } = new();

    public ObservableCollection<double> FlowPoints { get; } = new();
    public ISeries[] FlowSeries { get; }

    [ObservableProperty] private string _connectionStatus = "Disconnected";
    [ObservableProperty] private double _batteryLevel = 100;
    [ObservableProperty] private int _dosesRemaining = 200;
    [ObservableProperty] private string _statusMessage = "Prêt.";
    [ObservableProperty] private string _footer = "© InhalerDashboard - Demo WPF .NET 8";
    [ObservableProperty] private Color _connectionColor = Colors.Gray;

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

        _service.OnReading += (_, reading) =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Readings.Insert(0, reading);
                while (Readings.Count > 500) Readings.RemoveAt(Readings.Count - 1);

                FlowPoints.Add(reading.FlowRate);
                while (FlowPoints.Count > 120) FlowPoints.RemoveAt(0);

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
        };
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        StatusMessage = "Connexion à l'inhalateur…";
        try
        {
            await _service.StartAsync(_appCts.Token);
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
        StatusMessage = "Inhalation simulée.";
    }

    [RelayCommand]
    private void Reset()
    {
        _service.Reset();
        Readings.Clear();
        FlowPoints.Clear();
        ConnectionColor = Colors.Gray;
        ConnectionStatus = "Disconnected";
        BatteryLevel = 100;
        DosesRemaining = 200;
        StatusMessage = "Réinitialisé.";
    }
}