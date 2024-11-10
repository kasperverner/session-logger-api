namespace SessionLogger.Users;

public record UserResponse(Guid Id, string Name, string Email, Role Roles);