using Task = SessionLogger.Tasks.Task;

namespace SessionLogger.Users;

public class UserTask
{
    private UserTask() { }

    public UserTask(User user, Task task)
    {
        User = user;
        UserId = user.Id;
        
        Task = task;
        TaskId = task.Id;
    }
    
    public Guid UserId { get; init; }
    public User User { get; init; }

    public Guid TaskId { get; init; }
    public Task Task { get; init; }
}