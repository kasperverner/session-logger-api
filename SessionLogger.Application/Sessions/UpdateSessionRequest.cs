namespace SessionLogger.Sessions;

public record UpdateSessionRequest(Guid SessionId, Guid TaskId, string? Description = null, SessionState? State = null, DateTime? StartDate = null, DateTime? EndDate = null);