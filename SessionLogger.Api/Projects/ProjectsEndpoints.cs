using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Filters.Parameters;
using SessionLogger.Projects.Tasks;

namespace SessionLogger.Projects;

public static class ProjectsEndpoints
{
    public static IEndpointRouteBuilder MapProjectsEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/projects")
            .WithTags("Projects")
            .MapEndpoint<GetProjects>()
            .MapEndpoint<CreateProject>()
            .MapGroup("/{projectId:guid}")
            .AddEndpointFilter<ProjectIdFromRouteFilter>()
            .MapEndpoint<GetProject>()
            .MapEndpoint<UpdateProject>()
            .MapEndpoint<DeleteProject>()
            .MapTasksEndpoints();
        
        return application;
    }
}