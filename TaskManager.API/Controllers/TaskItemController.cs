using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Interfaces;
using TaskManager.Core.DTOs.Task;

namespace TaskManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/workspace/{workspaceId}/projects/{projectId}/tasks")]
    public class TaskItemController(ITaskItemService service) : ControllerBase
    {
        private readonly ITaskItemService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromRoute] Guid workspaceId, [FromRoute] Guid projectId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var tasks = await _service.GetAllTasksAsync(userId, workspaceId, projectId);

            return Ok(tasks);
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTasksById([FromRoute] Guid workspaceId, [FromRoute] Guid projectId, [FromRoute] Guid taskId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var task = await _service.GetTaskByIdAsync(userId, workspaceId, projectId, taskId);

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromRoute] Guid workspaceId, [FromRoute] Guid projectId, [FromBody] CreateTaskItemDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var newTask = await _service.CreateTaskAsync(userId, workspaceId, projectId, dto);

            return CreatedAtAction(
                actionName: nameof(GetTasksById),
                routeValues: new { workspaceId, projectId, taskId = newTask.Id },
                value: newTask
            );
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid workspaceId, [FromRoute] Guid projectId, [FromRoute] Guid taskId, [FromBody] UpdateTaskItemDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var updatedTask = await _service.UpdateTaskAsync(userId, workspaceId, projectId, taskId, dto);

            return Ok(updatedTask);
        }

        [HttpPut("{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus([FromRoute] Guid workspaceId, [FromRoute] Guid projectId, [FromRoute] Guid taskId, [FromBody] UpdateTaskItemStatusDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var updatedTask = await _service.UpdateTaskStatusAsync(userId, workspaceId, projectId, taskId, dto);

            return Ok(updatedTask);
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid workspaceId, [FromRoute] Guid projectId, [FromRoute] Guid taskId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            await _service.RemoveTaskAsync(userId, workspaceId, projectId, taskId);

            return NoContent();
        }
    }
}