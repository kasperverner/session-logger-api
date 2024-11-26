namespace SessionLogger.Users;

public interface IHaveUserId
{
    public Guid UserId { get; init; }
}