using TaskManager.Core.DTOs;
using TaskManager.Core.DTOs.Task;

namespace TaskManager.Core.Interfaces;

public interface ITaskItemService
{
    public Task<PageResult<TaskItemDto>> GetAllTasksAsync(string userId, Guid workspaceId, Guid projectId, TaskQueryParamsDto queryDto);
    public Task<TaskItemDto> GetTaskByIdAsync(string userId, Guid workspaceId, Guid projectId, Guid taskId);
    public Task<TaskItemDto> CreateTaskAsync(string userId, Guid workspaceId, Guid projectId, CreateTaskItemDto dto);
    public Task<TaskItemDto> UpdateTaskAsync(string userId, Guid workspaceId, Guid projectId, Guid taskId, UpdateTaskItemDto dto);
    public Task<TaskItemDto> UpdateTaskStatusAsync(string userId, Guid workspaceId, Guid projectId, Guid taskId, UpdateTaskItemStatusDto dto);
    public Task RemoveTaskAsync(string userId, Guid workspaceId, Guid projectId, Guid taskId);
}