using TaskManager.Core.DTOs.Comment;

namespace TaskManager.Core.Interfaces
{
    public interface ICommentService
    {
        public Task<IEnumerable<CommentDto>> GetAllCommentsAsync(string userId, Guid workspaceId, Guid TaskId);
        public Task<CommentDto> GetCommentByIdAsync(string userId, Guid workspaceId, Guid taskId, Guid commentId);
        public Task<CommentDto> CreateCommentAsync(string userId, Guid workspaceId, Guid taskId, CreateCommentDto dto);
        public Task<CommentDto> UpdateCommentAsync(string userId, Guid workspaceId, Guid taskId, Guid commentId, UpdateCommentDto dto);
        public Task RemoveCommentAsync(string userId, Guid workspaceId, Guid taskId, Guid commentId);
    }
}