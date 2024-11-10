using SessionLogger.Common;
using SessionLogger.Sessions;

namespace SessionLogger;

public record SessionResponse(Guid Id, Guid UserId, SessionType Type, Period Period, string? Description = null, SessionState? State = null, SessionTask? Task = null);
public record SessionTask(Guid Id, string Name, string? Description);