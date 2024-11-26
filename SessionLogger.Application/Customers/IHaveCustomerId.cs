namespace SessionLogger.Customers;

public interface IHaveCustomerId
{
    public Guid CustomerId { get; init; }
}