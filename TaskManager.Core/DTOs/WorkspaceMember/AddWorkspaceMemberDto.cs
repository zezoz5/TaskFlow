using System.ComponentModel.DataAnnotations;

namespace TaskManager.Core.DTOs.WorkspaceMember
{
    public class AddWorkspaceMemberDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}