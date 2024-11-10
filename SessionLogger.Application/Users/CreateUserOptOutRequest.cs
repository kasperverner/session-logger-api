namespace SessionLogger.Users;

public record CreateUserOptOutRequest(Guid UserId, DateTime StartDate, DateTime? EndDate = null);