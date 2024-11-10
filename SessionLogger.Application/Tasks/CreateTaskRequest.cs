namespace SessionLogger.Tasks;

public record CreateTaskRequest(Guid ProjectId, TaskType Type,  string Name, string? Description = null, DateTime? Deadline = null);