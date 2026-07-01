using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.DTOs.WorkspaceMember;
using TaskManager.Core.Interfaces;

namespace TaskManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/workspace/{workspaceId}/members")]
    public class WorkspaceMemberController(IWorkspaceMemberService service) : ControllerBase
    {
        private readonly IWorkspaceMemberService _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllMembers([FromRoute] Guid workspaceId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var membersDto = await _service.GetMembersAsync(userId, workspaceId);

            return Ok(membersDto);
        }

        [HttpPost]
        public async Task<IActionResult> Invite([FromRoute] Guid workspaceId, [FromBody] AddWorkspaceMemberDto dto)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var invitedMember = await _service.InviteAsync(userId, workspaceId, dto);

            return Ok(invitedMember);
        }

        [HttpPut("{targetUserId}")]
        public async Task<IActionResult> Promote([FromRoute] Guid workspaceId, [FromRoute] string targetUserId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var PromotedMember = await _service.PromoteAsync(userId, workspaceId, targetUserId);

            return Ok(PromotedMember);
        }

        [HttpDelete("{targetUserId}")]
        public async Task<IActionResult> Remove([FromRoute] Guid workspaceId, [FromRoute] string targetUserId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            await _service.RemoveAsync(userId, workspaceId, targetUserId);

            return NoContent();
        }
    }
}