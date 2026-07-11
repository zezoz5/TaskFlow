using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.WorkspaceMember;

public class WorkspaceMemberDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public WorkspaceRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}