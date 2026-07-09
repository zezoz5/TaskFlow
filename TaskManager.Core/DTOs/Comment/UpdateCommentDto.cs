using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.DTOs.Comment
{
    public class UpdateCommentDto
    {
        [MaxLength(500)]
        public string? Body { get; set; }
    }
}