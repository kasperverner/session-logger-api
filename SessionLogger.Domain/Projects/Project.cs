using SessionLogger.Tasks;

using Task = SessionLogger.Tasks.Task;

namespace SessionLogger.Projects;

public class Project : Entity
{
    private Project() { }
    
    public Project(Guid customerId, string name, string? description = null)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Name = name;
        Description = description;
    }
    
    public Guid CustomerId { get; init; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IList<Task> Tasks { get; init; } = new List<Task>();
    
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
        => Tasks.Add(new CompletableTask(Id, name, description, deadline));
    
    public void AddRecurringTask(string name, string? description = null)
        => Tasks.Add(new RecurringTask(Id, name, description));
}