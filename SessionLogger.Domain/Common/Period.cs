namespace SessionLogger.Common;

public class Period
{
    private Period() { }
    
    public Period(DateTime? start = null, DateTime? endDate = null)
    {
        StartDate = start ?? DateTime.UtcNow;
        EndDate = endDate;
    }
    
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; private set; }

    public void EndPeriod(DateTime? end = null)
    {
        if (EndDate.HasValue)
            return;
        
        EndDate = end ?? DateTime.UtcNow;
    }
}