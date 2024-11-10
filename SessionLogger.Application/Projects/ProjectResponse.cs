namespace SessionLogger.Projects;

public record ProjectResponse(Guid Id, Guid CustomerId, string Name, string? Description, bool HasActiveTasks);