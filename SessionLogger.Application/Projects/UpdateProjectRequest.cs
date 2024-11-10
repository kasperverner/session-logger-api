namespace SessionLogger.Projects;

public record UpdateProjectRequest(Guid Id, Guid CustomerId, string Name, string? Description = null);