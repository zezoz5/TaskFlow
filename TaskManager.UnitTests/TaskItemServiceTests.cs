using TaskManager.Core.DTOs.Task;
using TaskManager.Core.Enums;
using TaskManager.UnitTests.Helpers;

namespace TaskManager.UnitTests;

public class TaskItemServiceTests
{


    [Theory]
    [InlineData(TaskItemStatus.Todo)]
    [InlineData(TaskItemStatus.InProgress)]
    [InlineData(TaskItemStatus.Done)]
    public async Task GetAllTasksAsync_FilterByStatus_ReturnsMatchingTasks(TaskItemStatus status)
    {
        // Arrange
        var fixture = new TaskItemServiceFixture();

        var task1 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .WithStatus(TaskItemStatus.Todo)
            .Build();


        var task2 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .WithStatus(TaskItemStatus.InProgress)
            .Build();


        var task3 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .WithStatus(TaskItemStatus.Done)
            .Build();

        fixture.Context.TaskItems.AddRange(task1, task2, task3);
        await fixture.Context.SaveChangesAsync();

        var queryDto = new TaskQueryParamsDto { Status = status };

        // Act
        var result = await fixture.Sut.GetAllTasksAsync(fixture.User.Id, fixture.Workspace.Id, fixture.Project.Id, queryDto);

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
        var fixture = new TaskItemServiceFixture();

        var task1 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .WithPriority(TaskItemPriority.Low)
            .Build();

        var task2 = new TaskItemBuilder()
           .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .WithPriority(TaskItemPriority.Medium)
            .Build();

        var task3 = new TaskItemBuilder()
           .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .WithPriority(TaskItemPriority.High)
            .Build();

        fixture.Context.TaskItems.AddRange(task1, task2, task3);
        await fixture.Context.SaveChangesAsync();

        var queryDto = new TaskQueryParamsDto { Priority = priority };

        // Act
        var result = await fixture.Sut.GetAllTasksAsync(fixture.User.Id, fixture.Workspace.Id, fixture.Project.Id, queryDto);

        // Assert
        Assert.Equal(1, result.TotalCount);
        Assert.True(result.Items.All(t => t.Priority == priority));
    }

    [Fact]
    public async Task GetAllTasksAsync_Pagination_ReturnsCorrectPage()
    {
        // Arrange
        var fixture = new TaskItemServiceFixture();

        var task1 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var task2 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var task3 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var task4 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var task5 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var queryDto = new TaskQueryParamsDto
        {
            Page = 1,
            PageSize = 2
        };

        fixture.Context.TaskItems.AddRange(task1, task2, task3, task4, task5);
        await fixture.Context.SaveChangesAsync();

        // Act
        var result = await fixture.Sut.GetAllTasksAsync(fixture.User.Id, fixture.Workspace.Id, fixture.Project.Id, queryDto);

        // Assert
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.Items.Count());

    }

    [Fact]
    public async Task GetAllTasksAsync_Pagination_ReturnsSecondPage()
    {
        // Arrange
        var fixture = new TaskItemServiceFixture();

        var task1 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var task2 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var task3 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var task4 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var task5 = new TaskItemBuilder()
            .WithCreatorId(fixture.User.Id)
            .WithProjectId(fixture.Project.Id)
            .Build();

        var queryDto = new TaskQueryParamsDto
        {
            Page = 2,
            PageSize = 2
        };

        fixture.Context.TaskItems.AddRange(task1, task2, task3, task4, task5);
        await fixture.Context.SaveChangesAsync();

        // Act
        var result = await fixture.Sut.GetAllTasksAsync(fixture.User.Id, fixture.Workspace.Id, fixture.Project.Id, queryDto);

        // Assert
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.Items.Count());
    }
}