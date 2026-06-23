using TaskManager.Core.DTOs.Workspace;

namespace TaskManager.Core.Interfaces
{
    public interface IWorkspaceService
    {
        public Task<IEnumerable<WorkspaceDto>> GetAll(string userId);
        public Task<WorkspaceDto> GetById(Guid workspaceId, string userId);
        public Task<WorkspaceDto> CreateWorkspace(CreateWorkspaceDto dto, string userId);
        public Task<WorkspaceDto> UpdateWorkspace(Guid workspaceId, UpdateWorkspaceDto dto, string userId);
        public Task DeleteWorkspace(Guid workspaceId, string userId);
    }
}