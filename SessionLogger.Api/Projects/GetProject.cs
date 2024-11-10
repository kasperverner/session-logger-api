using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Projects;


public class GetProject : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get a specific project")
            .WithName("GetProject")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<ProjectResponse>();

    private static async Task<Ok<ProjectResponse>> Handle(
        [FromRoute] Guid projectId,
        [FromServices] IProjectService projectService,
        CancellationToken ct)
    {
        var response = await projectService.GetProjectAsync(projectId, ct);

        return TypedResults.Ok(response);
    }
}