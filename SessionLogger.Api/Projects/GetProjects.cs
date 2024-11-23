using FluentValidation;
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
            .WithRequestValidation<GetProjectsRequest>()
            .WithResponse<IEnumerable<ProjectResponse>>();

    public class RequestValidator : AbstractValidator<GetProjectsRequest>
    {
        public RequestValidator(ICustomerService customerService, ITaskService taskService, IUserService userService)
        {
            When(x => x.CustomerIds.Length > 0, () => RuleForEach(x => x.CustomerIds)
                .MustAsync(async (id, ct) => await customerService.CustomerExistsAsync(id, ct))
                .WithMessage("Customer does not exist"));

            When(x => x.UserIds.Length > 0, () => RuleForEach(x => x.UserIds)
                .MustAsync(async (id, ct) => await userService.UserExistsAsync(id, ct))
                .WithMessage("User does not exist"));
            
            When(x => x.EndDate.HasValue, () => RuleFor(x => x.EndDate)
                .Must((request, endDate) => !request.StartDate.HasValue || endDate > request.StartDate)
                .WithMessage("End date must come after start date"));
        }
    }

    
    private static async Task<Ok<IEnumerable<ProjectResponse>>> Handle(
        [AsParameters] GetProjectsRequest request,
        [FromServices] IProjectService projectService,
        CancellationToken ct)
    {
        var response = await projectService.GetProjectsAsync(request, ct);

        return TypedResults.Ok(response);
    }
}