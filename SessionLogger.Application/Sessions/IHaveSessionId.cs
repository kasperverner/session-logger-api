namespace SessionLogger.Sessions;

public interface IHaveSessionId
{
    public Guid SessionId { get; init; }
}