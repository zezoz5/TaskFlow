using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Services;
using TaskManager.Core.Enums;
using TaskManager.Core.DTOs.Project;
using TaskManager.Core.Exceptions;

namespace TaskManager.UnitTests;

public class ProjectServiceTests
{
    private readonly AppDbContext _context;
    private readonly ProjectService _sut;
    public ProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _sut = new ProjectService(_context);
    }

    // CreateProjectAsync
    [Fact]
    public async Task CreateProjectAsync_Manager_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var workspaceId = Guid.NewGuid();

        var workspace = new Workspace { Id = workspaceId, OwnerId = Guid.NewGuid().ToString(), Name = "Test Workspace" };
        _context.Workspaces.Add(workspace);

        var requesterUser = new AppUser { Id = userId, FullName = "Test User", Email = "Test@TaskFLow.com", UserName = "Test@TaskFLow.com" };
        _context.Users.Add(requesterUser);

        _context.WorkspaceMembers.Add(new WorkspaceMember { UserId = userId, WorkspaceId = workspaceId, Role = WorkspaceRole.Manager });

        var dto = new CreateProjectDto { Name = "Test project" };

        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.CreateProjectAsync(userId, workspaceId, dto);

        // Assert
        var exist = await _context.Projects.AnyAsync(p => p.WorkspaceId == workspaceId && p.Name == dto.Name);
        Assert.True(exist);

        Assert.Equal(dto.Name, result.Name);
    }

    [Fact]
    public async Task CreateProjectAsync_Member_Throws()
    {
        // Arrange
        Guid workspaceId = Guid.NewGuid();
        string userId = Guid.NewGuid().ToString();

        var workspace = new Workspace { Id = workspaceId, OwnerId = Guid.NewGuid().ToString(), Name = "Test Workspace" };
        _context.Workspaces.Add(workspace);

        var user = new AppUser { Id = userId, FullName = "Test User", UserName = "Test@TaskFlow.com", Email = "Test@TaskFlow.com" };
        _context.Users.Add(user);

        _context.WorkspaceMembers.Add(new WorkspaceMember { UserId = userId, WorkspaceId = workspaceId, Role = WorkspaceRole.Member });

        await _context.SaveChangesAsync();

        var dto = new CreateProjectDto { Name = "Test Project", Description = "Test Description", Status = ProjectStatus.Active };

        // Act + Assert
        var ex = await Assert.ThrowsAsync<AppException>(()
            => _sut.CreateProjectAsync(userId, workspaceId, dto));

        Assert.Equal("You don't have permission", ex.Message);
    }

    // UpdateProjectAsync
    [Fact]
    public async Task UpdateProjectAsync_PartialUpdate()
    {
        // Arrange
        Guid workspaceId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        string userId = Guid.NewGuid().ToString();

        var workspace = new Workspace { Id = workspaceId, OwnerId = userId, Name = "Test Workspace" };
        _context.Workspaces.Add(workspace);

        var user = new AppUser { Id = userId, FullName = "Test User", UserName = "Test@TaskFlow.com", Email = "Test@TaskFlow.com" };
        _context.Users.Add(user);

        _context.WorkspaceMembers.Add(new WorkspaceMember { UserId = userId, WorkspaceId = workspaceId, Role = WorkspaceRole.Owner });

        var project = new Project { Id = projectId, WorkspaceId = workspaceId, Name = "Backend API", Description = "Version 1", Status = ProjectStatus.Active };
        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        var dto = new UpdateProjectDto { Name = "TaskFlow API" };

        // Act
        var result = await _sut.UpdateProjectAsync(userId, workspaceId, projectId, dto);

        // Assert
        var updatedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        Assert.NotNull(updatedProject);

        Assert.Equal(dto.Name, result.Name);

        Assert.Equal(dto.Name, updatedProject.Name);
        Assert.Equal("Version 1", updatedProject.Description);
        Assert.Equal(ProjectStatus.Active, updatedProject.Status);
    }
}
