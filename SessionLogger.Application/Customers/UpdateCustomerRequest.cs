namespace SessionLogger.Customers;

public record UpdateCustomerRequest(Guid Id, Guid CustomerId, string Name);