using System;

namespace InhalerDashboard.Models;

public enum DeviceEventType
{
    Info,
    AirLeak,
    SensorFailure,
    LowBattery,
    BluetoothLost,
    BluetoothRestored
}

public sealed class DeviceEvent
{
    public DateTime Timestamp { get; init; }
    public DeviceEventType Type { get; init; }
    public string Message { get; init; } = "";
}
