namespace TaskManager.Core.DTOs.Workspace
{
    public class WorkspaceDto
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}