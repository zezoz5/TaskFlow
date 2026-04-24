namespace TaskManager.Core.Entities
{
    public class Workspace
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string OwnerId { get; set; } = null!;
        public AppUser Owner { get; set; } = null!;
        public ICollection<WorkspaceMember>? WorkspaceMembers { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }
}