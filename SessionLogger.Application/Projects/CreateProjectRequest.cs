namespace SessionLogger.Projects;

public record CreateProjectRequest(Guid CustomerId, string Name, string? Description = null);