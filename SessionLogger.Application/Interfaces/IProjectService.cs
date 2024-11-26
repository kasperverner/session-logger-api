using SessionLogger.Projects;

namespace SessionLogger.Interfaces;

public interface IProjectService
{
    Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken ct);
    Task<bool> ProjectExistsAsync(string name, Guid projectId, CancellationToken ct);
    Task<bool> ProjectExistsAsync(Guid customerId, string name, CancellationToken ct);
    Task<bool> ProjectExistsAsync(Guid[] customerIds, Guid projectId, CancellationToken ct);
    Task<IEnumerable<ProjectResponse>> GetProjectsAsync(GetProjectsRequest request, CancellationToken ct = default);
    Task<ProjectResponse> GetProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request, CancellationToken ct = default);
    Task UpdateProjectAsync(UpdateProjectRequest request, CancellationToken ct = default);
    Task DeleteProjectAsync(Guid projectId, CancellationToken ct = default);
}