using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTOs.Workspace;
using TaskManager.Core.Interfaces;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkspaceController(IWorkspaceService service) : ControllerBase
{
    private readonly IWorkspaceService _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAllWorkspaces()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null) return Unauthorized();

        var workspaces = await _service.GetAll(userId);
        return Ok(workspaces);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null) return Unauthorized();

        var workspace = await _service.GetById(id, userId);
        return Ok(workspace);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkspaceDto dto)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null) return Unauthorized();

        var workspace = await _service.CreateWorkspace(dto, userId);
        return CreatedAtAction(
            actionName: nameof(GetById),
            routeValues: new { id = workspace.Id },
            value: workspace
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWorkspaceDto dto)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null) return Unauthorized();

        var updatedWorkspace = await _service.UpdateWorkspace(id, dto, userId);
        return Ok(updatedWorkspace);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null) return Unauthorized();

        await _service.DeleteWorkspace(id, userId);

        return NoContent();
    }
}