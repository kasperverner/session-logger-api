using SessionLogger.Extensions;
using SessionLogger.Filters.Parameters;
using SessionLogger.Projects.Tasks.Comments;
using SessionLogger.Projects.Tasks.Users;

namespace SessionLogger.Projects.Tasks;

public static class TasksEndpoints
{
    public static IEndpointRouteBuilder MapTasksEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/tasks")
            .WithTags("Tasks")
            .MapEndpoint<GetTasks>()
            .MapEndpoint<CreateTask>()
            .MapGroup("/{taskId:guid}")
            .AddEndpointFilter<TaskIdFromRouteFilter>()
            .MapEndpoint<GetTask>()
            .MapEndpoint<UpdateTask>()
            .MapEndpoint<DeleteTask>()
            .MapCommentsEndpoints()
            .MapUsersEndpoints();
        
        return application;
    }
}