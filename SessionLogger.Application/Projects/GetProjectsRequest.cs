using Microsoft.AspNetCore.Mvc;

namespace SessionLogger.Projects;

public class GetProjectsRequest
{
    [FromQuery(Name = "customer_id")]
    public Guid[] CustomerIds { get; init; } = [];
    
    [FromQuery(Name = "user_id")]
    public Guid[] UserIds { get; init; } = [];
    
    [FromQuery(Name = "state")]
    public ProjectState[] ProjectStates { get; init; } = [];
    
    [FromQuery(Name = "start_date")]
    public DateTime? StartDate { get; init; }
    
    [FromQuery(Name = "end_date")]
    public DateTime? EndDate { get; init; }
}