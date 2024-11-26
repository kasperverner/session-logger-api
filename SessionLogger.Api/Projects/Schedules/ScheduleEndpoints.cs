using SessionLogger.Extensions;
using SessionLogger.Filters.Parameters;

namespace SessionLogger.Projects.Schedules;

public static class ScheduleEndpoints
{
    public static IEndpointRouteBuilder MapScheduleEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/schedule")
            .WithTags("Projects schedule")
            .MapEndpoint<GetSchedules>()
            .MapEndpoint<CreateSchedule>()
            .MapGroup("/{scheduleId:guid}")
            .AddEndpointFilter<ScheduleIdFromRouteFilter>()
            .MapEndpoint<GetSchedule>()
            .MapEndpoint<UpdateSchedule>()
            .MapEndpoint<DeleteSchedule>();
        
        return application;
    }
}