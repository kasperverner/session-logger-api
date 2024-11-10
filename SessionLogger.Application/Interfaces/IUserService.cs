using SessionLogger.Users;

namespace SessionLogger.Interfaces;

public interface IUserService
{
    Task<bool> UserExistsAsync(Guid userId, CancellationToken ct);
    Task<bool> UserExistsAsync(Guid userId, Role roles, CancellationToken ct);
    Task<bool> UserNameExistsAsync(string name, Guid userId, CancellationToken ct);
    Task<bool> UserEmailExistsAsync(string email, Guid userId, CancellationToken ct);
    
    Task<IEnumerable<UserResponse>> GetUsersAsync(CancellationToken ct);
    Task<UserResponse> GetUserAsync(Guid userId, CancellationToken ct);
    Task<UserResponse> GetAuthorizedUserAsync(CancellationToken ct);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct);
    Task UpdateUserAsync(UpdateUserRequest request, CancellationToken ct);
    Task DeleteUserAsync(Guid userId, CancellationToken ct);
    
    Task<bool> UserHasActiveOptOUtAsync(Guid optOutId, CancellationToken ct);
    Task<bool> OptOutExistsAsync(Guid optOutId, CancellationToken ct);
    Task<bool> OptOutStartsBeforeAsync(Guid optOutGuid, DateTime endDate, CancellationToken ct);
    
    Task<IEnumerable<OptOutResponse>> GetUserOptOutsAsync(Guid userId, CancellationToken ct);
    Task<OptOutResponse> CreateOptOutAsync(CreateUserOptOutRequest request, CancellationToken ct);
    Task UpdateOptOutAsync(UpdateUserOptOutRequest request, CancellationToken ct);
    Task DeleteOptOutAsync(Guid outOutId, CancellationToken ct);
}