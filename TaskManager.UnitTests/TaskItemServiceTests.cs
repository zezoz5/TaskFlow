using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.DTOs.Task;
using TaskManager.Core.Enums;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Services;
using TaskManager.UnitTests.helpers;
using TaskManager.UnitTests.Helpers;

namespace TaskManager.UnitTests;

public class TaskItemServiceTests
{
    private readonly AppDbContext _context;
    private readonly TaskItemService _sut;
    public TaskItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

        _context = new AppDbContext(options);

        _sut = new TaskItemService(_context);
    }

    [Theory]
    [InlineData(TaskItemStatus.Todo)]
    [InlineData(TaskItemStatus.InProgress)]
    [InlineData(TaskItemStatus.Done)]
    public async Task GetAllTasksAsync_FilterByStatus_ReturnsMatchingTasks(TaskItemStatus status)
    {
        // Arrange
        var user = TestDataHelper.CreateUser();
        _context.Users.Add(user);

        var workspace = TestDataHelper.CreateWorkspace();
        _context.Workspaces.Add(workspace);

        _context.WorkspaceMembers.Add(new WorkspaceMember
        {
            UserId = user.Id,
            WorkspaceId = workspace.Id,
            Role = WorkspaceRole.Member
        });

        var project = TestDataHelper.CreateProject();
        project.WorkspaceId = workspace.Id;
        _context.Projects.Add(project);

        var task1 = new TaskBuilder()
            .WithCreatorId(user.Id)
            .WithProjectId(project.Id)
            .WithStatus(TaskItemStatus.Todo)
            .Build();

        _context.TaskItems.Add(task1);

        var task2 = new TaskBuilder()
            .WithCreatorId(user.Id)
            .WithProjectId(project.Id)
            .WithStatus(TaskItemStatus.InProgress)
            .Build();

        _context.TaskItems.Add(task2);

        var task3 = new TaskBuilder()
            .WithCreatorId(user.Id)
            .WithProjectId(project.Id)
            .WithStatus(TaskItemStatus.Done)
            .Build();

        _context.TaskItems.Add(task3);
        await _context.SaveChangesAsync();

        var queryDto = new TaskQueryParamsDto { Status = status };

        // Act
        var result = await _sut.GetAllTasksAsync(user.Id, workspace.Id, project.Id, queryDto);

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.True(result.Items.All(t => t.Status == status));
    }

    [Theory]
    [InlineData(TaskItemPriority.Low)]
    [InlineData(TaskItemPriority.Medium)]
    [InlineData(TaskItemPriority.High)]
    public async Task GetAllTasksAsync_FilterByPriority_ReturnsMatchingTasks(TaskItemPriority priority)
    {
        // Arrange
        var user = TestDataHelper.CreateUser();
        _context.Users.Add(user);

        var workspace = TestDataHelper.CreateWorkspace();
        _context.Workspaces.Add(workspace);

        _context.WorkspaceMembers.Add(new WorkspaceMember { UserId = user.Id, WorkspaceId = workspace.Id, Role = WorkspaceRole.Member });

        var project = TestDataHelper.CreateProject();
        project.WorkspaceId = workspace.Id;
        _context.Projects.Add(project);

        var task1 = new TaskBuilder()
            .WithCreatorId(user.Id)
            .WithProjectId(project.Id)
            .WithPriority(TaskItemPriority.Low)
            .Build();

        var task2 = new TaskBuilder()
            .WithCreatorId(user.Id)
            .WithProjectId(project.Id)
            .WithPriority(TaskItemPriority.Medium)
            .Build();

        var task3 = new TaskBuilder()
            .WithCreatorId(user.Id)
            .WithProjectId(project.Id)
            .WithPriority(TaskItemPriority.High)
            .Build();

        _context.TaskItems.AddRange(task1, task2, task3);
        await _context.SaveChangesAsync();

        var queryDto = new TaskQueryParamsDto { Priority = priority };

        // Act
        var result = await _sut.GetAllTasksAsync(user.Id, workspace.Id, project.Id, queryDto);

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.True(result.Items.All(t => t.Priority == priority));
    }
}