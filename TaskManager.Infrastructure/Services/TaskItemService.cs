using Microsoft.EntityFrameworkCore;
using TaskManager.Core.DTOs.Task;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Services
{
    public class TaskItemService(AppDbContext context) : ITaskItemService
    {
        private readonly AppDbContext _context = context;
        public async Task<IEnumerable<TaskItemDto>> GetAllTasksAsync(string userId, Guid workspaceId, Guid projectId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            return await _context.TaskItems
                .Where(t => t.ProjectId == projectId)
                .Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    CreatorId = t.CreatorId,
                    AssignedToId = t.AssignedToId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    Deadline = t.Deadline,
                    CreatedAt = t.CreatedAt,
                })
                .ToListAsync();
        }

        public async Task<TaskItemDto> GetTaskByIdAsync(string userId, Guid workspaceId, Guid projectId, Guid taskId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId)
                ?? throw new AppException("Task not found", 404);

            return new TaskItemDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                CreatorId = task.CreatorId,
                AssignedToId = task.AssignedToId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                Deadline = task.Deadline,
                CreatedAt = task.CreatedAt,
            };
        }

        public async Task<TaskItemDto> CreateTaskAsync(string userId, Guid workspaceId, Guid projectId, CreateTaskItemDto dto)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            if (member.Role != Core.Enums.WorkspaceRole.Owner && member.Role != Core.Enums.WorkspaceRole.Manager)
                throw new AppException("You are not Authorized to create a task", 403);

            if (dto.AssignedToId != null)
            {
                var assignedToTask = await GetMemberAsync(dto.AssignedToId, workspaceId)
                    ?? throw new AppException("No member with this Id was be found", 400);
            }

            var newTask = new TaskItem
            {
                ProjectId = projectId,
                CreatorId = userId,
                AssignedToId = dto.AssignedToId,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Deadline = dto.Deadline,
            };

            _context.TaskItems.Add(newTask);
            await _context.SaveChangesAsync();

            return new TaskItemDto
            {
                Id = newTask.Id,
                ProjectId = newTask.ProjectId,
                CreatorId = newTask.CreatorId,
                AssignedToId = newTask.AssignedToId,
                Title = newTask.Title,
                Description = newTask.Description,
                Status = newTask.Status,
                Priority = newTask.Priority,
                Deadline = newTask.Deadline,
                CreatedAt = newTask.CreatedAt,
            };
        }

        public async Task<TaskItemDto> UpdateTaskAsync(string userId, Guid workspaceId, Guid projectId, Guid taskId, UpdateTaskItemDto dto)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            if (member.Role != Core.Enums.WorkspaceRole.Owner && member.Role != Core.Enums.WorkspaceRole.Manager)
                throw new AppException("You are not Authorized to Update a task", 403);

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new AppException("Task does not exist", 404);

            task.Title = dto.Title ?? task.Title;
            task.Description = dto.Description ?? task.Description;
            task.Deadline = dto.Deadline ?? task.Deadline;
            task.Priority = dto.Priority ?? task.Priority;

            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();

            return new TaskItemDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                CreatorId = task.CreatorId,
                AssignedToId = task.AssignedToId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                Deadline = task.Deadline,
                CreatedAt = task.CreatedAt,
            };
        }

        public async Task<TaskItemDto> UpdateTaskStatusAsync(string userId, Guid workspaceId, Guid projectId, Guid taskId, UpdateTaskItemStatusDto dto)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new AppException("Task does not exist", 404);

            task.Status = dto.Status;

            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();

            return new TaskItemDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                CreatorId = task.CreatorId,
                AssignedToId = task.AssignedToId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                Deadline = task.Deadline,
                CreatedAt = task.CreatedAt,
            };
        }

        public async Task RemoveTaskAsync(string userId, Guid workspaceId, Guid projectId, Guid taskId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            if (member.Role != Core.Enums.WorkspaceRole.Owner && member.Role != Core.Enums.WorkspaceRole.Manager)
                throw new AppException("You are not Authorized to Delete a task", 403);

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == taskId)
                ?? throw new AppException("Task does not exist", 404);

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
        }

        private async Task<WorkspaceMember?> GetMemberAsync(string userId, Guid workspaceId)
        {
            return await _context.WorkspaceMembers
            .FirstOrDefaultAsync(wm => wm.UserId == userId && wm.WorkspaceId == workspaceId);
        }
    }
}