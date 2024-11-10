using SessionLogger.Projects;

namespace SessionLogger.Customers;

public class Customer : Entity
{
    private Customer() { }
    
    public Customer(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    
    public string Name { get; private set; }
    
    public IList<Project> Projects { get; set; } = new List<Project>();

    public void UpdateName(string name)
    {
        if (Name == name)
            return;
        
        Name = name;
    }
    
    public void AddProject(string name, string? description = null)
        => Projects.Add(new Project(Id, name, description));
}