using TaskManager.Core.Enums;

namespace TaskManager.Core.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; }
        public TaskItemPriority Priority { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string CreatorId { get; set; } = null!;
        public AppUser TaskCreator { get; set; } = null!;
        public string? AssignedToId { get; set; }
        public AppUser? AssignedTo { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = [];
    }
}