using System.Text.Json.Serialization;
using SessionLogger.Projects;

namespace SessionLogger.Tasks;

public record UpdateTaskRequest(Guid Id, TaskType Type,  string Name, string? Description = null, TaskState? State = null, DateTime? Deadline = null) : IHaveProjectId
{
    [JsonIgnore]
    public Guid ProjectId { get; init; }
}