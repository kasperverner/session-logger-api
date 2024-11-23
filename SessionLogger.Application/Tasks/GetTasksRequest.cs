using Microsoft.AspNetCore.Mvc;

namespace SessionLogger.Tasks;

public class GetTasksRequest
{
    [FromRoute(Name = "projectId")] 
    public Guid ProjectId { get; init; }
    
    [FromQuery(Name = "state")] 
    public TaskState[] TaskStates { get; init; } = [];
    
    [FromQuery(Name = "user_id")]
    public Guid[] UserIds { get; init; } = [];
}