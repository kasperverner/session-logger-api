using SessionLogger.Common;

namespace SessionLogger.Schedules;

public abstract class Schedule : Entity
{
    protected Schedule() { }
    
    public Schedule(Period period)
    {
        Period = period;
    }
    public ScheduleType Type { get; init; }
    public Period Period { get; private set; }
    
    public void UpdatePeriod(Period period)
        => Period = period;
    
    public void UpdatePeriod(DateTime? startDate = null, DateTime? endDate = null) 
        => Period = new Period(startDate, endDate);

    public void EndPeriod(DateTime? endDate = null) 
        => Period.EndPeriod(endDate);
}