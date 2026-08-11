using TaskManager.Core.Entities;
using TaskManager.Core.Enums;

namespace TaskManager.UnitTests.Helpers
{
    public class TaskBuilder
    {
        private readonly TaskItem _task = new()
        {
            Title = "Test Task",
            CreatorId = "test-user-id",
            ProjectId = Guid.NewGuid()
        };

        public TaskBuilder WithCreatorId(string creatorId)
        {
            _task.CreatorId = creatorId;

            return this;
        }
        public TaskBuilder WithAssignedToId(string assignedToId)
        {
            _task.AssignedToId = assignedToId;

            return this;
        }
        public TaskBuilder WithProjectId(Guid projectId)
        {
            _task.ProjectId = projectId;

            return this;
        }

        public TaskBuilder WithTitle(string title)
        {
            _task.Title = title;

            return this;
        }
        public TaskBuilder WithDescription(string description)
        {
            _task.Description = description;

            return this;
        }
        public TaskBuilder WithStatus(TaskItemStatus status)
        {
            _task.Status = status;

            return this;
        }
        public TaskBuilder WithPriority(TaskItemPriority priority)
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