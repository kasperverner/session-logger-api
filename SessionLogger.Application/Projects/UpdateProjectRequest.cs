using System.Text.Json.Serialization;

namespace SessionLogger.Projects;

public record UpdateProjectRequest(string Name, string? Description = null) : IHaveProjectId
{
    [JsonIgnore]
    public Guid ProjectId { get; init; }
}