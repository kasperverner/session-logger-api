using SessionLogger.Customers;

namespace SessionLogger.Users;

public class Department : Entity
{
    private Department() { }
    
    public Department(string name, decimal rate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Name = name;
        Rates.Add(new DepartmentRate(this, rate));
    }
    
    public string Name { get; private set; }
    
    public IList<User> Users { get; init; } = new List<User>();
    
    public IList<DepartmentRate> Rates { get; init; } = new List<DepartmentRate>();
    
    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        if (Name == name)
            return;
        
        Name = name;
    }
    
    public void AddUser(User user)
    {
        if (Users.Contains(user))
            return;
        
        Users.Add(user);
    }
    
    public void RemoveUser(User user)
    {
        if (!Users.Contains(user))
            return;
        
        Users.Remove(user);
    }
    
    public void AddCustomerRate(Customer customer, decimal rate)
    {
        if (Rates.Any(x => x.Customer == customer))
            return;
        
        Rates.Add(new DepartmentRate(this, rate, customer));
    }
    
    public void UpdateCustomerRate(Customer customer, decimal rate)
    {
        var departmentRate = Rates.FirstOrDefault(x => x.Customer == customer);

        departmentRate?.UpdateRate(rate);
    }
    
    public void RemoveCustomerRate(Customer customer)
    {
        var departmentRate = Rates.FirstOrDefault(x => x.Customer == customer);

        if (departmentRate is null)
            return;
        
        Rates.Remove(departmentRate);
        departmentRate.Delete();
    }
}