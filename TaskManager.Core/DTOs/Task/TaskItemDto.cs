using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Task;

public class TaskItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskItemPriority Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }

    public string CreatorId { get; set; } = null!;
    public string? AssignedToId { get; set; }
    public Guid ProjectId { get; set; }
}