using FluentAssertions;
using SessionLogger.Customers;
using SessionLogger.Tests.Utilities;

namespace SessionLogger.Tests.UnitTests;

public class CustomerTests
{
    
    
    [Fact]
    public void CreateCustomer_ShouldInitializeProperties()
    {
        // Arrange
        var name = TestHelpers.Constants.Customers.Name;
        
        // Act
        var customer = new Customer(name);

        // Assert
        customer.Name.Should().Be(name);
    }
    
    [Fact]
    public void CreateCustomer_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var name = string.Empty;
        
        // Act
        var action = () =>
        {
            var customer = new Customer(name);
        };
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void UpdateName_ShouldUpdateName_WhenNameIsDifferent()
    {
        // Arrange
        var name = TestHelpers.Constants.Customers.Name;
        var customer = new Customer(name);
        var newName = "Ursa Minor Beta";
        
        // Act
        customer.UpdateName(newName);
        
        // Assert
        customer.Name.Should().Be(newName);
    }
    
    [Fact]
    public void UpdateName_ShouldNotUpdateName_WhenNameIsSame()
    {
        // Arrange
        var name = TestHelpers.Constants.Customers.Name;
        var customer = new Customer(name);
        
        // Act
        customer.UpdateName(name);
        
        // Assert
        customer.Name.Should().Be(name);
    }
    
    [Fact]
    public void UpdateName_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var name = TestHelpers.Constants.Customers.Name;
        var customer = new Customer(name);
        
        // Act
        var action = () =>
        {
            customer.UpdateName(string.Empty);
        };
        
        // Act and Assert
        action.Should().Throw<ArgumentException>();
    }
}