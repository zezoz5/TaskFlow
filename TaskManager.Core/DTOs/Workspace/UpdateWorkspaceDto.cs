using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.DTOs.Workspace;

public class UpdateWorkspaceDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
    public string? Description { get; set; }
}