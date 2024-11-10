namespace SessionLogger.Tasks;

public class CompletableTask : Task
{
    private CompletableTask()
    {
        Type = TaskType.Completable;
    }
    
    public CompletableTask(Guid projectId, string name, string? description = null, DateTime? deadline = null) : base(projectId, name, description)
    {
        Type = TaskType.Completable;
        
        Deadline = deadline;
        State = TaskState.Pending;
    }
    
    public TaskState State { get; private set; }
    public DateTime? Deadline { get; private set; }

    public void UpdateState(TaskState state)
    {
        if (State == state)
            return;
        
        State = state;
    }
    
    public void UpdateDeadline(DateTime? deadline)
    {
        if (State == TaskState.Completed || Deadline == deadline)
            return;
        
        Deadline = deadline;
    }
}