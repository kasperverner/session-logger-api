using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Projects;

// TODO: Modify

public class DeleteProject : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapDelete("", Handle)
            .WithSummary("Delete a specific project")
            .WithRequiredRoles(Role.Manager)
            .WithResponse();
    
    private static async Task<NoContent> Handle(
        [AsParameters] DeleteProjectRequest request, 
        [FromServices] IProjectService projectService,
        CancellationToken ct)
    {
        await projectService.DeleteProjectAsync(request.ProjectId, ct);
        
        return TypedResults.NoContent();
    }
}