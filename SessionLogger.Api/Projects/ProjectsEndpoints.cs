using SessionLogger.Extensions;
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
            .MapEndpoint<GetProject>()
            .MapEndpoint<UpdateProject>()
            .MapEndpoint<DeleteProject>()
            .MapTasksEndpoints();
        
        return application;
    }
}