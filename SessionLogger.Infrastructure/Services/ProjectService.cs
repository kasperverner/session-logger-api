using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SessionLogger.Exceptions;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;
using SessionLogger.Projects;
using SessionLogger.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SessionLogger.Infrastructure.Services;

public class ProjectService(ILogger<ProjectService> logger, SessionLoggerContext context) : IProjectService
{
    public async Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken ct)
    {
        return await context.Projects.AnyAsync(x => x.Id == projectId, ct);
    }

    public async Task<bool> ProjectExistsAsync(Guid customerId, string name, CancellationToken ct)
    {
        return await context.Projects.AnyAsync(x => x.CustomerId == customerId && x.Name == name, ct);
    }

    public async Task<bool> ProjectExistsAsync(Guid customerId, string name, Guid projectId, CancellationToken ct)
    {
        return await context.Projects.AnyAsync(x => x.CustomerId == customerId && x.Name == name && x.Id != projectId, ct);
    }

    public async Task<IEnumerable<ProjectResponse>> GetProjectsAsync(GetProjectsRequest request, CancellationToken ct = default)
    {
        var projectsQuery = context.Projects
            .AsNoTracking();
        
        if (request.CustomerId.HasValue)
            projectsQuery = projectsQuery.Where(x => x.CustomerId == request.CustomerId);
        
        var projects = await projectsQuery
            .Include(x => x.Tasks)
            .Select(x => new ProjectResponse(x.Id, x.CustomerId, x.Name, x.Description, x.Tasks
                .OfType<CompletableTask>()
                .Any(t => t.State != TaskState.Completed)))
            .ToListAsync(ct);

        return projects;
    }

    public async Task<ProjectResponse> GetProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var project = await context.Projects
            .AsNoTracking()
            .Include(x => x.Tasks)
            .Select(x => new ProjectResponse(x.Id, x.CustomerId, x.Name, x.Description, x.Tasks
                .OfType<CompletableTask>()
                .Any(t => t.State != TaskState.Completed)))
            .FirstOrDefaultAsync(x => x.Id == projectId, ct);
        
        if (project is null)
            throw new NotFoundException(nameof(Project), projectId);
        
        return project;
    }

    public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request, CancellationToken ct = default)
    {
        var project = new Project(request.CustomerId, request.Name, request.Description);
        
        await context.Projects.AddAsync(project, ct);
        await context.SaveChangesAsync(ct);
        
        logger.LogInformation("Created project {ProjectId}/{ProjectName}", project.Id, project.Name);
        
        return new ProjectResponse(project.Id, project.CustomerId, project.Name, project.Description, false);
    }

    public async Task UpdateProjectAsync(UpdateProjectRequest request, CancellationToken ct = default)
    {
        var project = await context.Projects.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        
        if (project is null)
            throw new NotFoundException(nameof(Project), request.Id);
        
        project.UpdateName(request.Name);
        project.UpdateDescription(request.Description);
        
        logger.LogInformation("Updated project {ProjectId}/{ProjectName}", project.Id, project.Name);

        context.Projects.Update(project);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var project = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId, ct);
        
        if (project is null)
            throw new NotFoundException(nameof(Project), projectId);
        
        project.Delete();
        
        logger.LogInformation("Delete project {ProjectId}/{ProjectName}", project.Id, project.Name);
        
        context.Projects.Update(project);
        await context.SaveChangesAsync(ct);
    }
}