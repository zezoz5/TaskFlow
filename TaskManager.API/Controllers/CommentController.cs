using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTOs.Comment;
using TaskManager.Core.Interfaces;

namespace TaskManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}/comments")]
    public class CommentController(ICommentService service) : ControllerBase
    {
        private readonly ICommentService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromRoute] Guid workspaceId, [FromRoute] Guid taskId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var comments = await _service.GetAllCommentsAsync(userId, workspaceId, taskId);

            return Ok(comments);
        }

        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetTaskById([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromRoute] Guid commentId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var comment = await _service.GetCommentByIdAsync(userId, workspaceId, taskId, commentId);

            return Ok(comment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromRoute] Guid projectId, [FromBody] CreateCommentDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var newComment = await _service.CreateCommentAsync(userId, workspaceId, taskId, dto);

            return CreatedAtAction(
                actionName: nameof(GetTaskById),
                routeValues: new { workspaceId, projectId, taskId, commentId = newComment.Id },
                value: newComment
                );
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromRoute] Guid commentId, [FromBody] UpdateCommentDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var comment = await _service.UpdateCommentAsync(userId, workspaceId, taskId, commentId, dto);

            return Ok(comment);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid workspaceId, [FromRoute] Guid taskId, [FromRoute] Guid commentId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            await _service.RemoveCommentAsync(userId, workspaceId, taskId, commentId);

            return NoContent();
        }
    }
}