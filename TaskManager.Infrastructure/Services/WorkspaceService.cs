using Microsoft.EntityFrameworkCore;
using TaskManager.Core.DTOs.Workspace;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Core.Enums;

namespace TaskManager.Infrastructure.Services
{
    public class WorkspaceService(AppDbContext context) : IWorkspaceService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<WorkspaceDto>> GetAll(string userId)
        {
            return await _context.WorkspaceMembers
                 .Where(u => u.UserId == userId)
                 .Select(w => new WorkspaceDto
                 {
                     Id = w.WorkspaceId,
                     OwnerId = w.Workspace.OwnerId,
                     Name = w.Workspace.Name,
                     Description = w.Workspace.Description,
                     CreatedAt = w.Workspace.CreatedAt
                 })
                 .ToListAsync();
        }

        public async Task<WorkspaceDto> GetById(Guid workspaceId, string userId)
        {
            var workspace = await _context.WorkspaceMembers.Include(i => i.Workspace)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.WorkspaceId == workspaceId);

            if (workspace != null)
            {
                return new WorkspaceDto
                {
                    Id = workspace.WorkspaceId,
                    OwnerId = workspace.Workspace.OwnerId,
                    Name = workspace.Workspace.Name,
                    Description = workspace.Workspace.Description,
                    CreatedAt = workspace.Workspace.CreatedAt
                };
            }

            throw new AppException("Workspace Not Found", 404);

        }

        public async Task<WorkspaceDto> CreateWorkspace(CreateWorkspaceDto dto, string userId)
        {
            var workspace = new Workspace
            {
                Name = dto.Name,
                Description = dto.Description,
                OwnerId = userId
            };

            var workspaceMember = new WorkspaceMember
            {
                UserId = userId,
                WorkspaceId = workspace.Id,
                Role = WorkspaceRole.Owner
            };

            _context.Workspaces.Add(workspace);
            _context.WorkspaceMembers.Add(workspaceMember);
            await _context.SaveChangesAsync();

            return new WorkspaceDto
            {
                Id = workspace.Id,
                OwnerId = userId,
                Name = workspace.Name,
                Description = workspace.Description,
                CreatedAt = workspace.CreatedAt
            };
        }

        public async Task<WorkspaceDto> UpdateWorkspace(Guid workspaceId, UpdateWorkspaceDto dto, string userId)
        {
            var member = await _context.WorkspaceMembers
            .Include(w => w.Workspace)
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId &&
                wm.UserId == userId &&
                (wm.Role == WorkspaceRole.Owner || wm.Role == WorkspaceRole.Manager));

            if (member?.Workspace != null)
            {
                member.Workspace.Name = dto.Name ?? member.Workspace.Name;
                member.Workspace.Description = dto.Description ?? member.Workspace.Description;

                _context.Workspaces.Update(member.Workspace);
                await _context.SaveChangesAsync();

                return new WorkspaceDto
                {
                    Id = member.Workspace.Id,
                    OwnerId = member.Workspace.OwnerId,
                    Name = member.Workspace.Name,
                    Description = member.Workspace.Description,
                    CreatedAt = member.Workspace.CreatedAt
                };
            }
            else
            {
                throw new AppException("Workspace Not Found", 404);
            }
        }

        public async Task DeleteWorkspace(Guid workspaceId, string userId)
        {
            var workspace = await _context.Workspaces.FirstOrDefaultAsync(w => w.Id == workspaceId && w.OwnerId == userId);

            if (workspace != null)
            {
                _context.Remove(workspace);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new AppException("Workspace Not Found", 404);
            }
        }
    }
}