using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.DTOs.Workspace
{
    public class UpdateWorkspaceDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}