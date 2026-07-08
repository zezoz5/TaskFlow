using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTOs.Project;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces;

namespace TaskManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/workspace/{workspaceId}/projects")]
    public class ProjectController(IProjectService service) : ControllerBase
    {
        private readonly IProjectService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllProjects([FromRoute] Guid workspaceId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var projects = await _service.GetAllProjectsAsync(userId, workspaceId);
            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById([FromRoute] Guid workspaceId, [FromRoute] Guid projectId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var project = await _service.GetProjectByIdAsync(userId, workspaceId, projectId);
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromRoute] Guid workspaceId, [FromBody] CreateProjectDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var newProject = await _service.CreateProjectAsync(userId, workspaceId, dto);
            return CreatedAtAction(
                actionName: nameof(GetProjectById),
                routeValues: new { workspaceId, projectId = newProject.Id },
                value: newProject
            );
        }

        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject([FromRoute] Guid workspaceId, [FromRoute] Guid projectId, [FromBody] UpdateProjectDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var updatedProject = await _service.UpdateProjectAsync(userId, workspaceId, projectId, dto);

            return Ok(updatedProject);
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject([FromRoute] Guid workspaceId, [FromRoute] Guid projectId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            await _service.RemoveProjectAsync(userId, workspaceId, projectId);

            return NoContent();
        }
    }
}