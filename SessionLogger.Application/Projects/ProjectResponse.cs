namespace SessionLogger.Projects;

public record ProjectResponse(Guid Id, Guid CustomerId, ProjectState State, string Name, string? Description, int ActiveTasks);