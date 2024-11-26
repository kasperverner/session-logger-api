using SessionLogger.Common;
using SessionLogger.Customers;
using SessionLogger.Schedules;
using SessionLogger.Tasks;
using SessionLogger.Users;
using Task = SessionLogger.Tasks.Task;

namespace SessionLogger.Projects;

public class Project : Entity
{
    private Project() { }
    
    public Project(Guid customerId, string name, string? description = null)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        State = ProjectState.Open;
        Name = name;
        Description = description;
    }
    
    public Guid CustomerId { get; init; }
    public ProjectState State { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IList<Task> Tasks { get; init; } = new List<Task>();
    public IList<ProjectSchedule> Schedule { get; init; } = new List<ProjectSchedule>();
    
    public void UpdateName(string name)
    {
        if (Name == name)
            return;
        
        Name = name;
    }

    public void UpdateDescription(string? description)
    {
        if (Description == description)
            return;
        
        Description = description;
    }
    
    public void AddCompletableTask(string name, string? description = null, DateTime? deadline = null)
        => Tasks.Add(new CompletableTask(this, name, description, deadline));
    
    public void AddRecurringTask(string name, string? description = null)
        => Tasks.Add(new RecurringTask(this, name, description));
    
    public ProjectSchedule? AddSchedule(ProjectSchedule projectSchedule)
    {
        if (Schedule.Contains(projectSchedule))
            return null;
        
        if (Schedule.Any(s => s.Period.Overlaps(projectSchedule.Period) && s.Department == projectSchedule.Department))
            throw new InvalidOperationException("Project already has a schedule for the department that overlaps with the provided schedule.");
        
        Schedule.Add(projectSchedule);
        
        return projectSchedule;
    }
    
    public ProjectSchedule? AddSchedule(Period period, Department department, int approvedHours, CustomerContact? approvedBy = null)
    {
        var projectSchedule = new ProjectSchedule(period, this, department, approvedHours, approvedBy);
        return AddSchedule(projectSchedule);
    }
}