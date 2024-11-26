using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SessionLogger.Customers;
using SessionLogger.Infrastructure.Services;
using SessionLogger.Persistence;
using SessionLogger.Tests.Utilities;

namespace SessionLogger.Tests.IntegrationTests;

[Collection("SessionLoggerContextTests")]
public class CustomerServiceTests
{
    private readonly SessionLoggerContext _context;
    private readonly CustomerService _sut;

    public CustomerServiceTests(SessionLoggerContextFixture fixture)
    {
        var logger = Mock.Of<ILogger<CustomerService>>();
        _context = fixture.Context;
        _sut = new CustomerService(logger, _context);
    }

    private async Task CleanupDatabase()
    {
        var customers = await _context.Customers.ToListAsync();
        _context.Customers.RemoveRange(customers);
        await _context.SaveChangesAsync();
    }
    
    [Fact]
    public async Task CustomerExists_WithValidId_ReturnsTrue()
    {
        try
        {
            // Arrange
            var customer = new Customer("Test Customer");
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Act
            var exists = await _sut.CustomerExistsAsync(customer.Id, CancellationToken.None);

            // Assert
            exists.Should().BeTrue();
        }
        finally
        {
            await CleanupDatabase();
        }
    }
}