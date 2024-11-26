using System.Text.Json.Serialization;
using SessionLogger.Projects;

namespace SessionLogger.Tasks;

public record CreateTaskRequest(TaskType Type,  string Name, string? Description = null, DateTime? Deadline = null) : IHaveProjectId
{
    [JsonIgnore]
    public Guid ProjectId { get; init; }
}