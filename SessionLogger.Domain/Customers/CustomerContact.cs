namespace SessionLogger.Customers;

public class CustomerContact : Entity
{
    private CustomerContact() { }
    
    public CustomerContact(Customer customer, string name, string email, string? title = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        
        CustomerId = customer.Id;
        Customer = customer;
        
        Name = name;
        Email = email;
        Title = title;
    }

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string? Title { get; private set; }
    
    public CustomerContact UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        
        if (Name != name) 
            Name = name;
            
        return this;
    }
    
    public CustomerContact UpdateEmail(string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        
        if (Email != email) 
            Email = email;
            
        return this;
    }
    
    public CustomerContact UpdateTitle(string? title)
    {
        if (Title != title) 
            Title = title;
            
        return this;
    }
}