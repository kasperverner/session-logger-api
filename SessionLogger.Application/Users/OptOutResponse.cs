using SessionLogger.Common;

namespace SessionLogger.Users;

public record OptOutResponse(Guid Id, Guid UserId, Period Period, bool IsActive);