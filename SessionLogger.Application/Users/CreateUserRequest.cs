namespace SessionLogger.Users;

public record CreateUserRequest(Guid PrincipalId, string Name, string Email, Role Roles = Role.None);