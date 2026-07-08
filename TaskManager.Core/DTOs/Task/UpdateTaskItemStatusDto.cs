using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Task
{
    public class UpdateTaskItemStatusDto
    {
        [Required]
        public TaskItemStatus Status { get; set; }
    }
}