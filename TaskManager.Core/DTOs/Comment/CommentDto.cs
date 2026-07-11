namespace TaskManager.Core.DTOs.Comment;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Body { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public Guid TaskItemId { get; set; }
    public string WriterId { get; set; } = null!;
}