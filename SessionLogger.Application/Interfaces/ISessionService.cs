using SessionLogger.Sessions;

namespace SessionLogger.Interfaces;

public interface ISessionService
{
    Task<bool> SessionExistsAsync(Guid sessionId, CancellationToken ct);
    Task<bool> SessionExistsAsync(Guid sessionId, SessionState state, CancellationToken ct);
    
    Task<IEnumerable<SessionResponse>> GetSessionsAsync(CancellationToken ct);
    Task<SessionResponse> GetSessionAsync(Guid sessionId, CancellationToken ct);
    Task<SessionResponse> CreateProjectSessionAsync(CreateSessionRequest request, CancellationToken ct);
    Task UpdateSessionAsync(UpdateSessionRequest request, CancellationToken ct);
    Task DeleteSessionAsync(Guid sessionId, CancellationToken ct);
}