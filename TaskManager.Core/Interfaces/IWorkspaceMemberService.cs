using TaskManager.Core.DTOs.WorkspaceMember;

namespace TaskManager.Core.Interfaces;

public interface IWorkspaceMemberService
{
    public Task<IEnumerable<WorkspaceMemberDto>> GetMembersAsync(string userId, Guid workspaceId);
    public Task<WorkspaceMemberDto> InviteMemberAsync(string userId, Guid workspaceId, AddWorkspaceMemberDto dto);
    public Task<WorkspaceMemberDto> PromoteMemberAsync(string userId, Guid workspaceId, string targetUserId);
    public Task RemoveMemberAsync(string userId, Guid workspaceId, string targetUserId);
}