using SessionLogger.Customers;
using SessionLogger.Users;

namespace SessionLogger.Interfaces;

public interface ICustomerService
{
    Task<bool> CustomerExistsAsync(Guid customerId, CancellationToken ct);
    Task<bool> CustomerExistsAsync(string name, CancellationToken ct);
    Task<bool> CustomerExistsAsync(string name, Guid customerId, CancellationToken ct);
    Task<IEnumerable<CustomerResponse>> GetCustomersAsync(CancellationToken ct = default);
    Task<CustomerResponse> GetCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<CustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken ct = default);
    Task UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken ct = default);
    Task DeleteCustomerAsync(Guid customerId, CancellationToken ct = default);
}