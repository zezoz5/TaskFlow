using TaskManager.Core.DTOs.Project;

namespace TaskManager.Core.Interfaces;

public interface IProjectService
{
    public Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(string userId, Guid workspaceId);
    public Task<ProjectDto> GetProjectByIdAsync(string userId, Guid workspaceId, Guid projectId);
    public Task<ProjectDto> CreateProjectAsync(string userId, Guid workspaceId, CreateProjectDto dto);
    public Task<ProjectDto> UpdateProjectAsync(string userId, Guid workspaceId, Guid projectId, UpdateProjectDto dto);
    public Task RemoveProjectAsync(string userId, Guid workspaceId, Guid projectId);
}