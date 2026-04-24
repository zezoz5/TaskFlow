using TaskManager.core.Enums;

namespace TaskManager.Core.Entities
{
    public class WorkspaceMember
    {
        public string UserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        public WorkspaceRole Role { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}