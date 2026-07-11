using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.DTOs.WorkspaceMember;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Core.Enums;

namespace TaskManager.Infrastructure.Services;

public class WorkspaceMemberService(AppDbContext context, UserManager<AppUser> user) : IWorkspaceMemberService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<AppUser> _user = user;
    public async Task<IEnumerable<WorkspaceMemberDto>> GetMembersAsync(string userId, Guid workspaceId)
    {
        var exist = await _context.WorkspaceMembers
        .AnyAsync(e => e.UserId == userId && e.WorkspaceId == workspaceId);

        if (exist)
        {
            return await _context.WorkspaceMembers.Where(w => w.WorkspaceId == workspaceId)
            .Select(wm => new WorkspaceMemberDto
            {
                UserId = wm.UserId,
                FullName = wm.AppUser.FullName,
                Email = wm.AppUser.Email!,
                Role = wm.Role,
                JoinedAt = wm.JoinedAt
            }).ToListAsync();
        }

        throw new AppException("You are not a member of this workspace", 403);
    }

    public async Task<WorkspaceMemberDto> InviteAsync(string userId, Guid workspaceId, AddWorkspaceMemberDto dto)
    {
        var requester = await _context.WorkspaceMembers
            .FirstOrDefaultAsync(m => m.UserId == userId && m.WorkspaceId == workspaceId)
            ?? throw new AppException("User not found", 404);

        if (requester.Role != WorkspaceRole.Owner && requester.Role != WorkspaceRole.Manager)
            throw new AppException("You don't have permission to invite members", 403);

        var invitedUser = await _user.FindByEmailAsync(dto.Email)
            ?? throw new AppException("User does not exist", 404);

        var alreadyMember = await _context.WorkspaceMembers
            .AnyAsync(m => m.UserId == invitedUser.Id && m.WorkspaceId == workspaceId);

        if (alreadyMember)
            throw new AppException("Already a member", 400);

        var workspaceMember = new WorkspaceMember
        {
            UserId = invitedUser.Id,
            WorkspaceId = workspaceId,
            Role = WorkspaceRole.Member
        };

        _context.WorkspaceMembers.Add(workspaceMember);
        await _context.SaveChangesAsync();

        return new WorkspaceMemberDto
        {
            UserId = invitedUser.Id,
            Email = invitedUser.Email!,
            FullName = invitedUser.FullName,
            Role = WorkspaceRole.Member,
            JoinedAt = workspaceMember.JoinedAt
        };
    }

    public async Task<WorkspaceMemberDto> PromoteAsync(string userId, Guid workspaceId, string targetUserId)
    {
        var requester = await _context.WorkspaceMembers.Include(wm => wm.AppUser)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.WorkspaceId == workspaceId)
            ?? throw new AppException("User not found", 404);

        if (requester.Role != WorkspaceRole.Owner)
            throw new AppException("You don't have permission to Promote", 403);

        var targetUser = await _context.WorkspaceMembers.Include(wm => wm.AppUser)
            .FirstOrDefaultAsync(t => t.UserId == targetUserId && t.WorkspaceId == workspaceId)
            ?? throw new AppException("User doesn't exist in this workspace", 404);

        if (targetUser.Role != WorkspaceRole.Member)
            throw new AppException("Can't promote this member", 400);

        targetUser.Role = WorkspaceRole.Manager;

        _context.WorkspaceMembers.Update(targetUser);
        await _context.SaveChangesAsync();

        return new WorkspaceMemberDto
        {
            UserId = targetUserId,
            Email = targetUser.AppUser.Email!,
            FullName = targetUser.AppUser.FullName,
            Role = targetUser.Role,
            JoinedAt = targetUser.JoinedAt
        };
    }

    public async Task RemoveAsync(string userId, Guid workspaceId, string targetUserId)
    {
        var requester = await _context.WorkspaceMembers.Include(wm => wm.AppUser)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.WorkspaceId == workspaceId)
            ?? throw new AppException("User not found", 404);

        var targetUser = await _context.WorkspaceMembers.Include(wm => wm.AppUser)
            .FirstOrDefaultAsync(t => t.UserId == targetUserId && t.WorkspaceId == workspaceId)
            ?? throw new AppException("User doesn't exist in this workspace", 404);

        if (requester.Role == WorkspaceRole.Owner && targetUser.Role != WorkspaceRole.Owner || requester.Role == WorkspaceRole.Manager && targetUser.Role == WorkspaceRole.Member)
        {
            _context.Remove(targetUser);
            await _context.SaveChangesAsync();
        }

        else
            throw new AppException("Can't remove this user", 403);
    }
}