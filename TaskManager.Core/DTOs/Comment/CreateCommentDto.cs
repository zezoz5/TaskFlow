using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.DTOs.Comment;

public class CreateCommentDto
{
    [Required]
    [MaxLength(500)]
    public string Body { get; set; } = null!;
}