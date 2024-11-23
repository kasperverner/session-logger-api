using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SessionLogger.Comments;
using SessionLogger.Exceptions;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;
using SessionLogger.Projects;
using SessionLogger.Tasks;
using SessionLogger.Users;
using Task = System.Threading.Tasks.Task;

namespace SessionLogger.Infrastructure.Services;

public class TaskService(ILogger<TaskService> logger, SessionLoggerContext context, IUserService userService): ITaskService
{
    public async Task<bool> TaskExistsAsync(Guid taskId, CancellationToken ct)
    {
        return await context.Tasks.AnyAsync(x => x.Id == taskId, ct);
    }

    public async Task<bool> TaskExistsAsync(Guid taskId, Guid userId, CancellationToken ct)
    {
        return await context.Tasks.AnyAsync(x => x.Id == taskId && x.AssignedUsers.Any(u => u.Id == userId), ct);
    }

    public async Task<bool> TaskExistsAsync(Guid projectId, string name, CancellationToken ct)
    {
        return await context.Tasks.AnyAsync(x => x.ProjectId == projectId && x.Name == name, ct);
    }

    public async Task<bool> TaskExistsAsync(Guid projectId, string name, Guid taskId, CancellationToken ct)
    {
        return await context.Tasks.AnyAsync(x => x.ProjectId == projectId && x.Name == name && x.Id != taskId, ct);
    }

    public async Task<bool> TaskExistsAsync(Guid[] projectIds, Guid taskId, CancellationToken ct)
    {
        return await context.Tasks.AnyAsync(x => projectIds.Contains(x.ProjectId) && x.Id == taskId, ct);
    }

    public async Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken ct)
    {
        SessionLogger.Tasks.Task task = request.Type switch
        {
            TaskType.Completable => await CreateCompletableTask(request, ct),
            TaskType.Recurring => await CreateRecurringTask(request, ct),
            _ => throw new ProblemException("Invalid task type", $"Task type {request.Type} is not supported.")
        };

        await context.Tasks.AddAsync(task, ct);
        await context.SaveChangesAsync(ct);
        
        return new TaskResponse(task.Id, task.Type, task.GetState(), task.Name, task.Description, 0, 0);
    }
    
    private async Task<CompletableTask> CreateCompletableTask(CreateTaskRequest request, CancellationToken ct)
        => new(await GetProjectAsync(request.ProjectId, ct), request.Name, request.Description, request.Deadline);

    private async Task<RecurringTask> CreateRecurringTask(CreateTaskRequest request, CancellationToken ct)
        => new(await GetProjectAsync(request.ProjectId, ct), request.Name, request.Description);
    

    private async Task<Project> GetProjectAsync(Guid projectId, CancellationToken ct)
    {
        var project = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId, ct);
        
        if (project is null)
            throw new NotFoundException(nameof(Project), projectId);
        
        return project;
    }
    
    public async Task<TaskResponse> GetTaskAsync(Guid taskId, CancellationToken ct)
    {
        var task = await context.Tasks
            .AsNoTracking()
            .Include(x => x.AssignedUsers)
            .Include(x => x.Comments)
            .Select(x => new TaskResponse(x.Id, x.Type, x.GetState(), x.Name, x.Description, x.AssignedUsers.Count, x.Comments.Count))
            .FirstOrDefaultAsync(x => x.Id == taskId, ct);

        if (task == null)
            throw new NotFoundException(nameof(SessionLogger.Tasks.Task), taskId);

        return task;
    }

    public async Task<IEnumerable<TaskResponse>> GetTasksAsync(Guid projectId, CancellationToken ct)
    {
        var tasks = await context.Tasks
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .Include(x => x.AssignedUsers)
            .Include(x => x.Comments)
            .Select(x => new TaskResponse(x.Id, x.Type, x.GetState(), x.Name, x.Description, x.AssignedUsers.Count, x.Comments.Count))
            .ToListAsync(ct);
        
        return tasks;
    }

    public async Task UpdateTaskAsync(UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        
        if (task is null)
            throw new NotFoundException(nameof(SessionLogger.Tasks.Task), request.Id);
        
        task.UpdateName(request.Name);
        task.UpdateDescription(request.Description);

        if (task is CompletableTask completableTask)
        {
            completableTask.UpdateDeadline(request.Deadline);
            
            if (request.State.HasValue)
                completableTask.UpdateState(request.State.Value);
        }
            
        logger.LogInformation("Updated task {TaskId}/{TaskName}", task.Id, task.Name);
        
        context.Tasks.Update(task);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken ct)
    {
        var task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == taskId, ct);
        
        if (task is null)
            throw new NotFoundException(nameof(SessionLogger.Tasks.Task), taskId);
        
        task.Delete();
        
        logger.LogInformation("Deleted task {TaskId}/{TaskName}", task.Id, task.Name);
        
        context.Tasks.Update(task);
        await context.SaveChangesAsync(ct);
    }

    public async Task AssignUserToTaskAsync(CreateUserTaskRequest request, CancellationToken ct)
    {
        var task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == request.TaskId, ct);
        
        if (task is null)
            throw new NotFoundException(nameof(SessionLogger.Tasks.Task), request.TaskId);
        
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, ct);
        
        if (user is null)
            throw new NotFoundException(nameof(User), request.UserId);
        
        task.AssignUser(user);
        
        logger.LogInformation("Assigned user {UserId} to task {TaskId}", user.Id, task.Id);
        
        context.Tasks.Update(task);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task<bool> CommentExistsAsync(Guid commentId, CancellationToken ct)
    {
        return await context.Comments.AnyAsync(x => x.Id == commentId, ct);
    }
    
    public async Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request, CancellationToken ct)
    {
        var task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == request.TaskId, ct);
        
        if (task is null)
            throw new NotFoundException(nameof(SessionLogger.Tasks.Task), request.TaskId);
        
        var authenticatedUser = await userService.GetAuthorizedUserAsync(ct);
        
        var user = await context.Users.FirstAsync(x => x.Id == authenticatedUser.Id, ct);
        var parent = await context.Comments.FirstOrDefaultAsync(x => x.Id == request.ParentId, ct);
        
        var comment = task.AddComment(user, request.Value, parent);

        logger.LogInformation("Created comment {CommentId} for task {TaskId}", task.Id, task.Name);
        
        context.Tasks.Update(task);
        await context.SaveChangesAsync(ct);
        
        return new CommentResponse(comment.Id, comment.ParentId, new AuthorResponse(user.Id, user.Name), comment.Value, comment.CreatedDate);
    }
        
    public async Task<IEnumerable<CommentResponse>> GetCommentsAsync(Guid taskId, CancellationToken ct)
    {
        var comments = await context.Comments
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.TaskId == taskId)
            .Select(x => new CommentResponse(x.Id, x.ParentId, new AuthorResponse(x.User.Id, x.User.Name), x.Value, x.CreatedDate))
            .ToListAsync(ct);
        
        return comments;
    }
    
    public async Task UpdateCommentAsync(UpdateCommentRequest request, CancellationToken ct)
    {
        var comment = await context.Comments.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        
        if (comment is null)
            throw new NotFoundException(nameof(Comment), request.Id);
        
        var authenticatedUser = await userService.GetAuthorizedUserAsync(ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != comment.UserId)
            throw new ForbiddenAccessException("You do not have permission to update this comment");
        
        comment.UpdateValue(request.Value);
        
        logger.LogInformation("Updated comment {CommentId} for task {TaskId}", comment.Id, comment.TaskId);
        
        context.Comments.Update(comment);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task DeleteCommentAsync(Guid commentId, CancellationToken ct)
    {
        var comment = await context.Comments.FirstOrDefaultAsync(x => x.Id == commentId, ct);
        
        if (comment is null)
            throw new NotFoundException(nameof(Comment), commentId);
        
        var authenticatedUser = await userService.GetAuthorizedUserAsync(ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != comment.UserId)
            throw new ForbiddenAccessException("You do not have permission to delete this comment");
        
        comment.Delete();
        
        logger.LogInformation("Deleted comment {CommentId} for task {TaskId}", comment.Id, comment.TaskId);
        
        context.Comments.Update(comment);
        await context.SaveChangesAsync(ct);
    }
}