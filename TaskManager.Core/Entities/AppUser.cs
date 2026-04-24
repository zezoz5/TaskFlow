using Microsoft.AspNetCore.Identity;

namespace TaskManager.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<WorkspaceMember>? WorkspaceMembers { get; set; }
        public ICollection<TaskItem>? CreatedTasks { get; set; }
        public ICollection<TaskItem>? AssignedTasks { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}