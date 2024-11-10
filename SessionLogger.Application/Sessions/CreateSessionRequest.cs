namespace SessionLogger.Sessions;

public record CreateSessionRequest(Guid TaskId, string? Description = null, DateTime? StartDate = null, DateTime? EndDate = null);