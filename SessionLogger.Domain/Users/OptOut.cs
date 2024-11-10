using SessionLogger.Common;

namespace SessionLogger.Users;

public class OptOut : Entity
{
    private OptOut() { }
    
    public OptOut(Guid userId, DateTime? start = null, DateTime? end = null)
    {
        UserId = userId;
        Period = new Period(start, end);
    }
    
    public Guid UserId { get; init; }
    public Period Period { get; init; }

    public void EndOptOut(DateTime? end = null)
        => Period.EndPeriod(end);
}