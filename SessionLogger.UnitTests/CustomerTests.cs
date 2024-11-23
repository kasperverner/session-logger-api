using FluentAssertions;
using SessionLogger.Customers;

namespace SessionLogger.UnitTests;

public class CustomerTests
{
    private const string Name = "Megadodo Publications";
    
    [Fact]
    public void CreateCustomer_ShouldInitializeProperties()
    {
        // Arrange
        // Act
        var customer = new Customer(Name);

        // Assert
        customer.Name.Should().Be(Name);
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
        var customer = new Customer(Name);
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
        var customer = new Customer(Name);
        
        // Act
        customer.UpdateName(Name);
        
        // Assert
        customer.Name.Should().Be(Name);
    }
    
    [Fact]
    public void UpdateName_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var customer = new Customer(Name);
        
        // Act
        var action = () =>
        {
            customer.UpdateName(string.Empty);
        };
        
        // Act and Assert
        action.Should().Throw<ArgumentException>();
    }
}