using SessionLogger.Users;

namespace SessionLogger.Interfaces;

public interface IUserService
{
    Task<bool> UserExistsAsync(Guid userId, CancellationToken ct);
    Task<bool> UserExistsAsync(Guid userId, Role roles, CancellationToken ct);
    Task<IEnumerable<UserResponse>> GetUsersAsync(CancellationToken ct);
    Task<UserResponse> GetUserAsync(Guid userId, CancellationToken ct);
    Task<UserResponse> GetAuthorizedUserAsync(CancellationToken ct);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct);
    Task UpdateUserAsync(UpdateUserRequest request, CancellationToken ct);
    Task DeleteUserAsync(Guid userId, CancellationToken ct);
}