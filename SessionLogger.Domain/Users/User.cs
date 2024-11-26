using SessionLogger.Common;
using SessionLogger.Schedules;
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
    
    public Guid? DepartmentId { get; private set; }
    public Department? Department { get; private set; }
    public IList<Session> Sessions { get; init; } = new List<Session>();
    public IList<Task> AssignedTasks { get; init; } = new List<Task>();
    public IList<UserSchedule> Schedule { get; init; } = new List<UserSchedule>();

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
    
    public void AssignDepartment(Department department)
    {
        if (DepartmentId == department.Id)
            return;
        
        Department = department;
        DepartmentId = department.Id;
    }
    
    public void RemoveDepartment()
    {
        Department = null;
        DepartmentId = null;
    }

    public void AddSession(Session session)
    {
        if (Sessions.Contains(session))
            return;
        
        Sessions.Add(session);
    }

    public void AssignRoles(Role roles)
        => Roles = roles;

    public void AssignTask(Task task)
    {
        if (AssignedTasks.Contains(task))
            return;
        
        AssignedTasks.Add(task);
    }

    public void RemoveTask(Task task)
    {
        if (!AssignedTasks.Contains(task))
            return;
        
        AssignedTasks.Remove(task);
    }
    
    public void AddSchedule(UserSchedule userSchedule)
    {
        if (Schedule.Contains(userSchedule))
            return;
        
        if (Schedule.Any(s => s.Period.Overlaps(userSchedule.Period)))
            throw new InvalidOperationException("User already has a schedule that overlaps with the provided schedule.");
        
        Schedule.Add(userSchedule);
    }
    
    public void AddSchedule(Period period, AbsenceType absenceType)
    {
        var userSchedule = new UserSchedule(period, this, absenceType);
        
        AddSchedule(userSchedule);
    }
}