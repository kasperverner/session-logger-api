using Microsoft.AspNetCore.Mvc;

namespace SessionLogger.Sessions;

public class GetSessionsRequest
{   
    [FromQuery(Name = "customer_id")]
    public Guid[] CustomerIds { get; init; } = [];
    
    [FromQuery(Name = "project_id")]
    public Guid[] ProjectIds { get; init; } = [];
    
    [FromQuery(Name = "task_id")]
    public Guid[] TaskIds { get; init; } = [];
    
    [FromQuery(Name = "user_id")]
    public Guid[] UserIds { get; init; } = [];
    
    [FromQuery(Name = "state")]
    public SessionState[] SessionStates { get; init; } =[];
    
    [FromQuery(Name = "start_date")]
    public DateTime? StartDate { get; init; }
    
    [FromQuery(Name = "end_date")]
    public DateTime? EndDate { get; init; }
}
