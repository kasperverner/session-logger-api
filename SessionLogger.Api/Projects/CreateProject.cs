using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Projects;

// TODO: Modify

public class CreateProject : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPost("", Handle)
            .WithSummary("Create a new project")
            .WithRequiredRoles(Role.Manager)
            .WithRequestValidation<CreateProjectRequest>()
            .WithResponse<ProjectResponse>();

    public class RequestValidator : AbstractValidator<CreateProjectRequest>
    {
        public RequestValidator(ICustomerService customerService, IProjectService projectService)
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await customerService.CustomerExistsAsync(id, ct))
                .WithMessage("Customer does not exist");
            
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(120)
                .MustAsync(async (request, name, ct) => !await projectService.ProjectExistsAsync(request.CustomerId, name, ct))
                .WithMessage("Project name already exists for this customer");
            
            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
    
    private static async Task<CreatedAtRoute<ProjectResponse>> Handle(
        [FromBody] CreateProjectRequest request,
        [FromServices] IProjectService projectService,
        CancellationToken ct)
    {
        var response = await projectService.CreateProjectAsync(request, ct);

        return TypedResults.CreatedAtRoute(response, "GetProject", new { projectId = response.Id });
    }
}
