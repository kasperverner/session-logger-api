using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SessionLogger.Customers;
using SessionLogger.Exceptions;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;
using SessionLogger.Users;

namespace SessionLogger.Infrastructure.Services;

public class CustomerService(ILogger<CustomerService> logger, SessionLoggerContext context) : ICustomerService
{
    public Task<bool> CustomerExistsAsync(Guid customerId, CancellationToken ct)
    {
        return context.Customers.AnyAsync(x => x.Id == customerId, ct);
    }
    
    public Task<bool> CustomerExistsAsync(string name, CancellationToken ct)
    {
        return context.Customers.AnyAsync(x => x.Name == name, ct);
    }

    public Task<bool> CustomerExistsAsync(string name, Guid customerId, CancellationToken ct)
    {
        return context.Customers.AnyAsync(x => x.Name == name && x.Id != customerId, ct);
    }

    public async Task<IEnumerable<CustomerResponse>> GetCustomersAsync(CancellationToken ct = default)
    {
        var customers = await context.Customers
            .AsNoTracking()
            .Select(x => new CustomerResponse(x.Id, x.Name))
            .ToListAsync(ct);

        return customers;
    }
    
    public async Task<CustomerResponse> GetCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        var customer = await context.Customers
            .AsNoTracking()
            .Select(x => new CustomerResponse(x.Id, x.Name))
            .FirstOrDefaultAsync(x => x.Id == customerId, ct);

        if (customer is null)
            throw new NotFoundException(nameof(Customer), customerId);
        
        return customer;
    }
    
    public async Task<CustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        var customer = new Customer(request.Name);
        
        await context.Customers.AddAsync(customer, ct);
        await context.SaveChangesAsync(ct);
        
        logger.LogInformation("Created customer {CustomerId}/{CustomerName}", customer.Id, customer.Name);
        
        return new CustomerResponse(customer.Id, customer.Name);
    }

    public async Task UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken ct = default)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        
        if (customer is null)
            throw new NotFoundException(nameof(Customer), request.Id);
        
        customer.UpdateName(request.Name);
        
        logger.LogInformation("Updated customer {CustomerId}/{CustomerName}", customer.Id, customer.Name);

        context.Customers.Update(customer);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task DeleteCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(x => x.Id == customerId, ct);
        
        if (customer is null)
            throw new NotFoundException(nameof(Customer), customerId);
        
        customer.Delete();
        
        logger.LogInformation("Delete customer {CustomerId}/{CustomerName}", customer.Id, customer.Name);
        
        context.Customers.Update(customer);
        await context.SaveChangesAsync(ct);
    }
}