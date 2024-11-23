using SessionLogger.Sessions;
using Task = SessionLogger.Tasks.Task;

namespace SessionLogger.Users;

public class User : Entity
{
    private User() { }
    
    public User(Guid principalId, string name, string email)
    {
        if (principalId == Guid.Empty)
            throw new ArgumentException("PrincipalId cannot be empty.", nameof(principalId));

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        PrincipalId = principalId;
        Name = name;
        Email = email;
        Roles = Role.None;
    }
    
    public Guid PrincipalId { get; init; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Role Roles { get; private set; }
    
    public IList<OptOut> OptOuts { get; init; } = new List<OptOut>();
    public IList<Session> Sessions { get; init; } = new List<Session>();
    public IList<Task> AssignedTasks { get; init; } = new List<Task>();

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        if (Name == name)
            return;
        
        Name = name;
    }

    public void UpdateEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        
        if (Email == email)
            return;
        
        Email = email;
    }
    
    public OptOut AddOptOut(DateTime? start = null, DateTime? end = null)
    {
        EndOpenOptOuts();
        
        var optOut = new OptOut(Id, start, end);
        
        OptOuts.Add(optOut);
        
        return optOut;
    }

    public void EndOpenOptOuts(DateTime? end = null)
    {
        var openOptOuts = OptOuts.Where(x => !x.Period.EndDate.HasValue);
        
        foreach (var optOut in openOptOuts)
            optOut.EndOptOut();
    }

    public void AddCheckInSession(DateTime? start = null, DateTime? end = null)
        => Sessions.Add(new CheckInSession(Id, start, end));

    public void AddProjectSession(Task task, string? description = null, DateTime? start = null, DateTime? end = null)
        => Sessions.Add(new ProjectSession(Id, task, description, start, end));

    public void AssignRoles(Role roles)
        => Roles = roles;
    
    public void AssignTask(Task task)
        => AssignedTasks.Add(task);
}