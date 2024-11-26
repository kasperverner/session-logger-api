using System.Text.Json.Serialization;

namespace SessionLogger.Projects;

public record DeleteProjectRequest() : IHaveProjectId
{
    [JsonIgnore]
    public Guid ProjectId { get; init; }
}