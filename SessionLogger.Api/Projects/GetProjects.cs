using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Projects;

public class GetProjects : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get all projects for a specific customer")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<IEnumerable<ProjectResponse>>();

    public record Request(Guid? CustomerIds);
    
    private static async Task<Ok<IEnumerable<ProjectResponse>>> Handle(
        [AsParameters] GetProjectsRequest request,
        [FromServices] IProjectService projectService,
        CancellationToken ct)
    {
        var response = await projectService.GetProjectsAsync(request, ct);

        return TypedResults.Ok(response);
    }
}