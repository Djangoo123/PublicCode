namespace InhalerDashboard.Models;

public enum Pathology
{
    None,
    Asthma,
    COPD 
}

public sealed class PatientProfile
{
    public int Age { get; init; }            
    public double LungCapacityLiters { get; init; } 
    public Pathology Pathology { get; init; }

    public double PeakFlowFactor
    {
        get
        {
            double factor = 1.0;

            if (Age > 65) factor *= 0.85;
            if (Pathology == Pathology.Asthma) factor *= 0.8;
            if (Pathology == Pathology.COPD) factor *= 0.7;

            return factor;
        }
    }
}