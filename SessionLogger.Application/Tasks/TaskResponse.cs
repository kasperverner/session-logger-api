namespace SessionLogger.Tasks;

public record TaskResponse(Guid Id, TaskType Type, TaskState? State, string Name, string? Description, int AssignedUsers, int Comments);