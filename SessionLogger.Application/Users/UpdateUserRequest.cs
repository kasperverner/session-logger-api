namespace SessionLogger.Users;

public record UpdateUserRequest(Guid Id, Role Roles);