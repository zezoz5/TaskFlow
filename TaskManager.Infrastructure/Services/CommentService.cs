using Microsoft.EntityFrameworkCore;
using TaskManager.Core.DTOs.Comment;
using TaskManager.Core.Entities;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Services
{
    public class CommentService(AppDbContext context) : ICommentService
    {
        private readonly AppDbContext _context = context;
        public async Task<IEnumerable<CommentDto>> GetAllCommentsAsync(string userId, Guid workspaceId, Guid TaskId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            return await _context.Comments
                .Where(c => c.TaskItemId == TaskId)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    TaskItemId = c.TaskItemId,
                    WriterId = c.WriterId,
                    Body = c.Body,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();
        }

        public async Task<CommentDto> GetCommentByIdAsync(string userId, Guid workspaceId, Guid taskId, Guid commentId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.TaskItemId == taskId && c.Id == commentId)
                ?? throw new AppException("This comment does not exist", 404);

            return new CommentDto
            {
                Id = comment.Id,
                TaskItemId = comment.TaskItemId,
                WriterId = comment.WriterId,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<CommentDto> CreateCommentAsync(string userId, Guid workspaceId, Guid taskId, CreateCommentDto dto)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            var comment = new Comment
            {
                Body = dto.Body,
                TaskItemId = taskId,
                WriterId = userId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                TaskItemId = comment.TaskItemId,
                WriterId = comment.WriterId,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<CommentDto> UpdateCommentAsync(string userId, Guid workspaceId, Guid taskId, Guid commentId, UpdateCommentDto dto)
        {
            var member = await GetMemberAsync(userId, workspaceId)
                ?? throw new AppException("You are not a member of this workspace", 403);

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.TaskItemId == taskId && c.Id == commentId)
                ?? throw new AppException("This comment does not exist", 404);

            if (comment.WriterId != userId) throw new AppException("You can not edit this comment", 403);

            comment.Body = dto.Body ?? comment.Body;

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                TaskItemId = comment.TaskItemId,
                WriterId = comment.WriterId,
                Body = comment.Body,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task RemoveCommentAsync(string userId, Guid workspaceId, Guid taskId, Guid commentId)
        {
            var member = await GetMemberAsync(userId, workspaceId)
               ?? throw new AppException("You are not a member of this workspace", 403);

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.TaskItemId == taskId && c.Id == commentId)
                ?? throw new AppException("This comment does not exist", 404);

            if (comment.WriterId != userId && member.Role != Core.Enums.WorkspaceRole.Owner && member.Role != Core.Enums.WorkspaceRole.Manager)
                throw new AppException("You can not delete this comment", 403);

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        private async Task<WorkspaceMember?> GetMemberAsync(string userId, Guid workspaceId)
        {
            return await _context.WorkspaceMembers
                .FirstOrDefaultAsync(wm => wm.UserId == userId && wm.WorkspaceId == workspaceId);
        }
    }
}