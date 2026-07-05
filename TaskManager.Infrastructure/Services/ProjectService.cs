using Microsoft.EntityFrameworkCore;
using TaskManager.Core.DTOs.Project;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Core.Enums;

namespace TaskManager.Infrastructure.Services
{
    public class ProjectService(AppDbContext context) : IProjectService
    {
        private readonly AppDbContext _context = context;
        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(string userId, Guid workspaceId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            return await _context.Projects
                .Where(p => p.WorkspaceId == workspaceId)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    WorkspaceId = p.WorkspaceId,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ProjectDto> GetProjectByIdAsync(string userId, Guid workspaceId, Guid projectId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.WorkspaceId == workspaceId && p.Id == projectId)
                ?? throw new AppException("Project not found", 404);

            return new ProjectDto
            {
                Id = project.Id,
                WorkspaceId = project.WorkspaceId,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                CreatedAt = project.CreatedAt
            };
        }

        public async Task<ProjectDto> CreateProjectAsync(string userId, Guid workspaceId, CreateProjectDto dto)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            if (member.Role != WorkspaceRole.Owner && member.Role != WorkspaceRole.Manager)
                throw new AppException("You don't have permission", 403);

            var project = new Project
            {
                WorkspaceId = workspaceId,
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                WorkspaceId = project.WorkspaceId,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                CreatedAt = project.CreatedAt
            };
        }

        public async Task<ProjectDto> UpdateProjectAsync(string userId, Guid workspaceId, Guid projectId, UpdateProjectDto dto)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            if (member.Role != WorkspaceRole.Owner && member.Role != WorkspaceRole.Manager)
                throw new AppException("You don't have permission", 403);

            var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.WorkspaceId == workspaceId && p.Id == projectId)
                ?? throw new AppException("Project not found", 404);

            project.Name = dto.Name ?? project.Name;
            project.Description = dto.Description ?? project.Description;
            if (dto.Status != null) project.Status = dto.Status.Value;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                WorkspaceId = project.WorkspaceId,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                CreatedAt = project.CreatedAt
            };
        }

        public async Task RemoveProjectAsync(string userId, Guid workspaceId, Guid projectId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            if (member.Role != WorkspaceRole.Owner && member.Role != WorkspaceRole.Manager)
                throw new AppException("You don't have permission", 403);

            var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.WorkspaceId == workspaceId && p.Id == projectId)
                ?? throw new AppException("Project not found", 404);

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        private async Task<WorkspaceMember?> GetMemberAsync(string userId, Guid workspaceId)
        {
            return await _context.WorkspaceMembers
                .FirstOrDefaultAsync(wm => wm.UserId == userId && wm.WorkspaceId == workspaceId);
        }
    }
}