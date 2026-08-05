using TaskManager.Core.DTOs.Workspace;

namespace TaskManager.Core.Interfaces;

public interface IWorkspaceService
{
    public Task<IEnumerable<WorkspaceDto>> GetAllWorkspacesAsync(string userId);
    public Task<WorkspaceDto> GetWorkspaceByIdAsync(Guid workspaceId, string userId);
    public Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceDto dto, string userId);
    public Task<WorkspaceDto> UpdateWorkspaceAsync(Guid workspaceId, UpdateWorkspaceDto dto, string userId);
    public Task RemoveWorkspaceAsync(Guid workspaceId, string userId);
}