using System.Text.Json.Serialization;
using SessionLogger.Projects;

namespace SessionLogger.Comments;

public record CreateCommentRequest(Guid TaskId, string Value, Guid? ParentId = null) : IHaveProjectId
{
    [JsonIgnore]
    public Guid ProjectId { get; init; }
}