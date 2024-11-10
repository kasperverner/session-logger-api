using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Exceptions;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Projects;

public class UpdateProject : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPut("", Handle)
            .WithSummary("Update a specific project")
            .WithRequiredRoles(Role.Manager)
            .WithRequestValidation<UpdateProjectRequest>()
            .WithResponse();
    
    public class RequestValidator : AbstractValidator<UpdateProjectRequest>
    {
        public RequestValidator(ICustomerService customerService, IProjectService projectService)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(async (id, ct) => await projectService.ProjectExistsAsync(id, ct))
                .WithMessage("Project does not exist");
            
            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await customerService.CustomerExistsAsync(id, ct))
                .WithMessage("Customer does not exist");
            
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(120)
                .MustAsync(async (request, name, ct) => !await projectService.ProjectExistsAsync(request.CustomerId, name, request.Id, ct))
                .WithMessage("Project name already exists for this customer");
            
            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
    
    private static async Task<NoContent> Handle(
        [FromRoute] Guid projectId,
        [FromBody] UpdateProjectRequest request,
        [FromServices] IProjectService projectService, 
        CancellationToken ct)
    {
        if (projectId != request.Id)
            throw new ProblemException("Project ID mismatch", "The project ID in the request body does not match the project ID in the URL");
        
        await projectService.UpdateProjectAsync(request, ct);
        
        return TypedResults.NoContent();
    }
}