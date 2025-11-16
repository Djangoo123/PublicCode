using System;

namespace InhalerDashboard.Models;

public sealed class InhalationSummary
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public double DurationSeconds => (End - Start).TotalSeconds;
    public double VolumeMl { get; init; }    
}