using TaskManager.Core.DTOs.WorkspaceMember;

namespace TaskManager.Core.Interfaces
{
    public interface IWorkspaceMemberService
    {
        public Task<IEnumerable<WorkspaceMemberDto>> GetMembersAsync(string userId, Guid workspaceId);
        public Task<WorkspaceMemberDto> InviteAsync(string userId, Guid workspaceId, AddWorkspaceMemberDto dto);
        public Task<WorkspaceMemberDto> PromoteAsync(string userId, Guid workspaceId, string targetUserId);
        public Task RemoveAsync(string userId, Guid workspaceId, string targetUserId);
    }
}