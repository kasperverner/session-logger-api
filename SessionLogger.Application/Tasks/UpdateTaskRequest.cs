namespace SessionLogger.Tasks;

public record UpdateTaskRequest(Guid Id, Guid ProjectId, TaskType Type,  string Name, string? Description = null, TaskState? State = null, DateTime? Deadline = null);