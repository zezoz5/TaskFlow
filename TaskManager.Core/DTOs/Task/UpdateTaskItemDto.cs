using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Task;

public class UpdateTaskItemDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskItemPriority? Priority { get; set; }
    public DateTime? Deadline { get; set; }
}