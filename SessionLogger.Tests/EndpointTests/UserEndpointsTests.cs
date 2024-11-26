using SessionLogger.Tests.Utilities;

namespace SessionLogger.Tests.EndpointTests;

[Collection("SessionLoggerContextTests")]
public class UserEndpointsTests : IClassFixture<SessionLoggerApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly SessionLoggerApplicationFactory _factory;
    private readonly SessionLoggerContextFixture _fixture;

    public UserEndpointsTests(
        SessionLoggerApplicationFactory factory,
        SessionLoggerContextFixture fixture)
    {
        _factory = factory;
        _fixture = fixture;
        _client = factory.CreateClient();
    }

    // [Fact]
    // public async Task GetUsers_ReturnsSuccessStatusCode()
    // {
    //     // Act
    //     var response = await _client.GetAsync("/api/users");
    //     var users = await response.Content.ReadFromJsonAsync<List<User>>();
    //
    //     // Assert
    //     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //     Assert.NotNull(users);
    //     Assert.NotEmpty(users);
    // }
    //
    // [Fact]
    // public async Task CreateUser_WithValidData_ReturnsCreatedResponse()
    // {
    //     // Arrange
    //     var newUser = new CreateUserRequest
    //     {
    //         Name = "John Doe",
    //         Email = "john@example.com"
    //     };
    //
    //     // Act
    //     var response = await _client.PostAsJsonAsync("/api/users", newUser);
    //     var createdUser = await response.Content.ReadFromJsonAsync<User>();
    //
    //     // Assert
    //     Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    //     Assert.NotNull(createdUser);
    //     Assert.Equal(newUser.Name, createdUser.Name);
    // }
    //
    // [Fact]
    // public async Task GetUser_WithInvalidId_ReturnsNotFound()
    // {
    //     // Arrange
    //     var invalidId = Guid.NewGuid();
    //
    //     // Act
    //     var response = await _client.GetAsync($"/api/users/{invalidId}");
    //
    //     // Assert
    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }
}