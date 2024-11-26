using Task = SessionLogger.Tasks.Task;

namespace SessionLogger.Sessions;

public class ProjectSession : Session
{
    private ProjectSession() {  }

    public ProjectSession(Guid userId, Task task, string? description = null, DateTime? startDate = null, DateTime? endDate = null) : base(userId, startDate, endDate)
    {
        TaskId = task.Id;
        Task = task;
        
        Description = description;
        
        State = SessionState.Open;
    }

    public Guid TaskId { get; init; }
    public Task Task { get; init; }
    public SessionState State { get; set; }
    public string? Description { get; private set; }
    public string? CommitUrl { get; private set; }
    
    public void UpdateState(SessionState state)
    {
        if (State == state || State == SessionState.Billed)
            return;
        
        State = state;
    }
    
    public void UpdateDescription(string? description)
    {
        if (State == SessionState.Billed || Description == description)
            return;
        
        Description = description;
    }
    
    public void UpdateCommitUrl(string? commitUrl)
    {
        if (State == SessionState.Billed || CommitUrl == commitUrl)
            return;
        
        CommitUrl = commitUrl;
    }
}