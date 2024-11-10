using SessionLogger.Common;

namespace SessionLogger.Sessions;

public abstract class Session : Entity
{
    protected Session() { }

    public Session(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        
        Period = new Period(startDate, endDate);
    }
    
    public Guid UserId { get; init; }
    public SessionType Type { get; init; }
    public Period Period { get; private set; }
    
    public void UpdatePeriod(DateTime? startDate = null, DateTime? endDate = null) 
        => Period = new Period(startDate, endDate);

    public void EndSession(DateTime? endDate = null) 
        => Period.EndPeriod(endDate);
}