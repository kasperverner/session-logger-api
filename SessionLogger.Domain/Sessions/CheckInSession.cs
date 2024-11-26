namespace SessionLogger.Sessions;

public class CheckInSession : Session
{
    private CheckInSession() { }

    public CheckInSession(Guid userId, DateTime? startDate = null, DateTime? endDate = null) : base(userId, startDate, endDate) { }
}