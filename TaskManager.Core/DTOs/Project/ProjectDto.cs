using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Project;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid WorkspaceId { get; set; }
}