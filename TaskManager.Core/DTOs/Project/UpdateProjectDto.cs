using System.ComponentModel.DataAnnotations;
using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Project;

public class UpdateProjectDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? Status { get; set; }
}