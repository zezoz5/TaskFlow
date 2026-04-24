namespace TaskManager.Core.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Body { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;
        public string WriterId { get; set; } = null!;
        public AppUser Writer { get; set; } = null!;
    }
}