namespace SessionLogger.Sessions;

public class CheckInSession : Session
{
    private CheckInSession()
    {
        Type = SessionType.CheckIn;
    }

    public CheckInSession(Guid userId, DateTime? startDate = null, DateTime? endDate = null) : base(userId, startDate, endDate)
    {
        Type = SessionType.CheckIn;
    }
}