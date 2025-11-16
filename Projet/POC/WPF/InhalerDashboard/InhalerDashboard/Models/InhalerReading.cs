using System;

namespace InhalerDashboard.Models;

public enum DeviceConnectionStatus
{
    Disconnected,
    Connecting,
    Connected
}

public sealed class InhalerReading
{
    public DateTime Timestamp { get; init; }
    public double FlowRate { get; init; } 
    public int DoseCount { get; init; }
    public double BatteryLevel { get; init; }
    public DeviceConnectionStatus ConnectionStatus { get; init; }
    public int? InhalationId { get; init; }
}