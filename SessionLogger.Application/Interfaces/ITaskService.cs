using SessionLogger.Comments;
using SessionLogger.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SessionLogger.Interfaces;

public interface ITaskService
{
    Task<bool> TaskExistsAsync(Guid taskId, CancellationToken ct);
    Task<bool> TaskExistsAsync(Guid taskId, Guid userId, CancellationToken ct);
    Task<bool> TaskExistsAsync(Guid projectId, string name, CancellationToken ct);
    Task<bool> TaskExistsAsync(Guid projectId, string name, Guid taskId, CancellationToken ct);
    Task<bool> TaskExistsAsync(Guid[] projectIds, Guid taskId, CancellationToken ct);
    
    Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken ct);
    Task<TaskResponse> GetTaskAsync(Guid taskId, CancellationToken ct);
    Task<IEnumerable<TaskResponse>> GetTasksAsync(Guid projectId, CancellationToken ct);
    Task UpdateTaskAsync(UpdateTaskRequest request, CancellationToken ct);
    Task DeleteTaskAsync(Guid taskId, CancellationToken ct);
    
    // Task AssignUserToTaskAsync(CreateUserTaskRequest request, CancellationToken ct);

    Task<bool> CommentExistsAsync(Guid commentId, CancellationToken ct);
    Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request, CancellationToken ct);
    Task<IEnumerable<CommentResponse>> GetCommentsAsync(Guid taskId, CancellationToken ct);
    Task UpdateCommentAsync(UpdateCommentRequest request, CancellationToken ct);
    Task DeleteCommentAsync(Guid commentId, CancellationToken ct);
}