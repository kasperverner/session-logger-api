using FluentAssertions;
using SessionLogger.Users;

namespace SessionLogger.Tests.UnitTests;

public class UserTests
{
    private const string Name = "John Doe";
    private const string Email = "johndoe@example.tld";
    
    [Fact]
    public void CreateUser_ShouldInitializeProperties()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        
        // Act
        var user = new User(principalId, Name, Email);

        // Assert
        user.PrincipalId.Should().Be(principalId);
        user.Name.Should().Be(Name);
        user.Email.Should().Be(Email);
    }

    [Fact]
    public void CreateUser_ShouldThrowArgumentException_WhenPrincipalIdIsEmpty()
    {
        // Arrange
        var principalId = Guid.Empty;

        // Act
        var action = () =>
        {
            var user = new User(principalId, Name, Email);
        };
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void CreateUser_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        
        // Act
        var action = () =>
        {
            var user = new User(principalId, string.Empty, Email);
        };
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void CreateUser_ShouldThrowArgumentException_WhenEmailIsEmpty()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        
        // Act
        var action = () =>
        {
            var user = new User(principalId, Name, string.Empty);
        };
        
        // Act and Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void AssignRoles_ShouldUpdateRoles_WhenRolesAreAdded()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var roles = Role.Employee | Role.Manager;
        var user = new User(principalId, Name, Email);
        
        // Act
        user.AssignRoles(roles);
        
        // Act and Assert
        user.Roles.Should().Be(roles);
        user.Roles.Should().HaveFlag(Role.Employee);
        user.Roles.Should().HaveFlag(Role.Manager);
    }

    [Fact]
    public void AssignRoles_ShouldUpdateRoles_WhenRolesAreRemoved()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var roles = Role.Employee | Role.Manager;
        var user = new User(principalId, Name, Email);
        
        // Act
        user.AssignRoles(roles);
        user.AssignRoles(user.Roles & ~Role.Manager);
        
        // Act and Assert
        user.Roles.Should().Be(Role.Employee);
        user.Roles.Should().HaveFlag(Role.Employee);
        user.Roles.Should().NotHaveFlag(Role.Manager);
    }
    
    [Fact]
    public void Delete_ShouldMarkUserAsDeleted()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var user = new User(principalId, Name, Email);
        
        // Act
        user.Delete();
        
        // Assert
        user.DeletedDate.Should().HaveValue();
    }
    
    [Fact]
    public void UpdateName_ShouldUpdateName_WhenNameIsDifferent()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var user = new User(principalId, Name, Email);
        var newName = "Jane Doe";
        
        // Act
        user.UpdateName(newName);
        
        // Assert
        user.Name.Should().Be(newName);
    }
    
    [Fact]
    public void UpdateName_ShouldNotUpdateName_WhenNameIsSame()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var user = new User(principalId, Name, Email);
        
        // Act
        user.UpdateName(Name);
        
        // Assert
        user.Name.Should().Be(Name);
    }
    
    [Fact]
    public void UpdateName_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var user = new User(principalId, Name, Email);
        
        // Act
        var action = () =>
        {
            user.UpdateName(string.Empty);
        };
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateEmail_ShouldUpdateEmail_WhenEmailIsDifferent()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var user = new User(principalId, Name, Email);
        var newEmail = "janedoe@example.tld";

        // Act
        user.UpdateEmail(newEmail);

        // Assert
        user.Email.Should().Be(newEmail);
    }
    
    [Fact]
    public void UpdateEmail_ShouldNotUpdateEmail_WhenEmailIsSame()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var user = new User(principalId, Name, Email);
        
        // Act
        user.UpdateEmail(Email);
        
        // Assert
        user.Email.Should().Be(Email);
    }
    
    [Fact]
    public void UpdateEmail_ShouldThrowArgumentException_WhenEmailIsEmpty()
    {
        // Arrange
        var principalId = Guid.NewGuid();
        var user = new User(principalId, Name, Email);
        
        // Act
        var action = () =>
        {
            user.UpdateEmail(string.Empty);
        };
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
}