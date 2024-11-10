namespace SessionLogger.Users;

public record UpdateUserOptOutRequest(Guid Id, DateTime EndDate);