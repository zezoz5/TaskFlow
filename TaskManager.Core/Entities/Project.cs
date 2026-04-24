using TaskManager.core.Enums;

namespace TaskManager.Core.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;
        public ICollection<TaskItem> Tasks { get; set; } = [];
    }
}