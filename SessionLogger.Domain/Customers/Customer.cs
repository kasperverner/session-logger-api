using SessionLogger.Projects;
using SessionLogger.Users;

namespace SessionLogger.Customers;

public class Customer : Entity
{
    private Customer() { }
    
    public Customer(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = Guid.NewGuid();
        Name = name;
    }
    
    public string Name { get; private set; }
    
    public IList<CustomerContact> Contacts { get; init; } = new List<CustomerContact>();
    public IList<Project> Projects { get; init; } = new List<Project>();
    public IList<DepartmentRate> Rates { get; init; } = new List<DepartmentRate>();

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        if (Name == name)
            return;
        
        Name = name;
    }
    
    public void AddProject(string name, string? description = null)
        => Projects.Add(new Project(Id, name, description));

    public void AddContact(string name, string email, string? phone = null)
    {
        if (Contacts.Any(x => x.Email == email))
            return;
        
        Contacts.Add(new CustomerContact(this, name, email, phone));
    }
    
    public void RemoveContact(Guid contactId)
    {
        var contact = Contacts.FirstOrDefault(x => x.Id == contactId);
        if (contact is null)
            return;
        
        Contacts.Remove(contact);
    }
    
    public void AddRate(Department department, decimal rate)
    {
        if (Rates.Any(x => x.Department == department))
            return;
        
        Rates.Add(new DepartmentRate(department, rate, this));
    }
}