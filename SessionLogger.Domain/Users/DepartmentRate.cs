using SessionLogger.Customers;

namespace SessionLogger.Users;

public class DepartmentRate : Entity
{
    private DepartmentRate() { }
    
    public DepartmentRate(Department department, decimal rate, Customer? customer = null)
    {
        DepartmentId = department.Id;
        Department = department;
        Rate = rate;
    }
    
    public Guid DepartmentId { get; init; }
    public Department Department { get; init; }
    
    public Guid? CustomerId { get; init; }
    public Customer? Customer { get; init; }
    
    public decimal Rate { get; private set; }
    
    public void UpdateRate(decimal rate)
    {
        if (Rate == rate)
            return;
        
        Rate = rate;
    }
}