using SessionLogger.Extensions;
using SessionLogger.Projects.Tasks.Comments;

namespace SessionLogger.Projects.Tasks;

public static class TasksEndpoints
{
    public static IEndpointRouteBuilder MapTasksEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/tasks")
            .MapEndpoint<GetTasks>()
            .MapEndpoint<CreateTask>()
            .MapGroup("/{taskId:guid}")
            .MapEndpoint<GetTask>()
            .MapEndpoint<UpdateTask>()
            .MapEndpoint<DeleteTask>()
            .MapEndpoint<CreateUserTask>()
            .MapCommentsEndpoints();
        
        return application;
    }
}