using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.UnitTests.Helpers
{
    public class TaskItemBuilder
    {
        private readonly TaskItem _task = new()
        {
            Title = "Test Task",
            CreatorId = "test-user-id",
            ProjectId = Guid.NewGuid()
        };

        public TaskItemBuilder WithCreatorId(string creatorId)
        {
            _task.CreatorId = creatorId;

            return this;
        }
        public TaskItemBuilder WithAssignedToId(string assignedToId)
        {
            _task.AssignedToId = assignedToId;

            return this;
        }
        public TaskItemBuilder WithProjectId(Guid projectId)
        {
            _task.ProjectId = projectId;

            return this;
        }

        public TaskItemBuilder WithTitle(string title)
        {
            _task.Title = title;

            return this;
        }
        public TaskItemBuilder WithDescription(string description)
        {
            _task.Description = description;

            return this;
        }
        public TaskItemBuilder WithStatus(TaskItemStatus status)
        {
            _task.Status = status;

            return this;
        }
        public TaskItemBuilder WithPriority(TaskItemPriority priority)
        {
            _task.Priority = priority;

            return this;
        }
        public TaskItem Build()
        {
            return _task;
        }
    }
}