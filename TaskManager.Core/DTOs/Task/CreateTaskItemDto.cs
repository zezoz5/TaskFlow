using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Task
{
    public class CreateTaskItemDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskItemPriority Priority { get; set; }
        public DateTime? Deadline { get; set; }
        public string? AssignedToId { get; set; }
    }
}