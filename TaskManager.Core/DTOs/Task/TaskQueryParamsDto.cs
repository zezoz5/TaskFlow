using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Task;

public class TaskQueryParamsDto
{
    public TaskItemStatus? Status { get; set; }
    public TaskItemPriority? Priority { get; set; }
    public DateTime? DeadlineBefore { get; set; }
    public string? AssignedToId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}