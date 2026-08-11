using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Services;
using TaskManager.Core.Enums;
using TaskManager.Core.DTOs.Project;
using TaskManager.Core.Exceptions;
using TaskManager.UnitTests.helpers;

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

    [Fact]
    public async Task UpdateProjectAsync_ProjectNotFound_Throws()
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

        await _context.SaveChangesAsync();

        var dto = new UpdateProjectDto { Name = "TaskFlow API" };

        // Act + Assert
        var ex = await Assert.ThrowsAsync<AppException>(()
            => _sut.UpdateProjectAsync(userId, workspaceId, projectId, dto));

        Assert.Equal("Project not found", ex.Message);
    }

    // RemoveProjectAsync
    [Fact]
    public async Task RemoveProjectAsync_Manager_Succeeds()
    {
        // Arrange
        Guid workspaceId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        string userId = Guid.NewGuid().ToString();

        var workspace = new Workspace { Id = workspaceId, OwnerId = Guid.NewGuid().ToString(), Name = "Test Workspace" };
        _context.Workspaces.Add(workspace);

        var user = new AppUser { Id = userId, FullName = "Test User", UserName = "Test@TaskFlow.com", Email = "Test@TaskFlow.com" };
        _context.Users.Add(user);

        _context.WorkspaceMembers.Add(new WorkspaceMember { UserId = userId, WorkspaceId = workspaceId, Role = WorkspaceRole.Manager });

        var project = new Project { Id = projectId, WorkspaceId = workspaceId, Name = "Backend API", Description = "Version 1", Status = ProjectStatus.Active };
        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        // Act
        await _sut.RemoveProjectAsync(userId, workspaceId, projectId);

        // Assert
        var stillExist = await _context.Projects.AnyAsync(p => p.Id == projectId);
        Assert.False(stillExist);
    }

    // GetAllProjectsAsync
    // Old manual setup — replaced below with TestDataHelper
    [Fact]
    public async Task GetAllProjectsAsync_ReturnsProjects()
    {
        // Arrange
        // Guid workspaceId = Guid.NewGuid();
        // string userId = Guid.NewGuid().ToString();
        // Guid projectId1 = Guid.NewGuid();
        // Guid projectId2 = Guid.NewGuid();
        // Guid projectId3 = Guid.NewGuid();

        /*
        var workspace = new Workspace
        {
            Id = workspaceId,
            OwnerId = Guid.NewGuid().ToString(),
            Name = "Test Workspace"
        };
        */
        var workspace = TestDataHelper.CreateWorkspace();
        _context.Workspaces.Add(workspace);

        /*
        var user = new AppUser
        {
            Id = userId,
            FullName = "Test User",
            UserName = "Test@TaskFlow.com",
            Email = "Test@TaskFlow.com"
        };
        */

        var user = TestDataHelper.CreateUser();
        _context.Users.Add(user);

        _context.WorkspaceMembers.Add(new WorkspaceMember
        {
            UserId = user.Id,
            WorkspaceId = workspace.Id,
            Role = WorkspaceRole.Member
        });

        /*
        var project1 = new Project
        {
            Id = projectId1,
            WorkspaceId = workspaceId,
            Name = "Backend API",
            Description = "Version 1",
            Status = ProjectStatus.Active
        };

        var project2 = new Project
        {
            Id = projectId2,
            WorkspaceId = workspaceId,
            Name = "Frontend",
            Description = "Version 1",
            Status = ProjectStatus.Active
        };

        var project3 = new Project
        {
            Id = projectId3,
            WorkspaceId = workspaceId,
            Name = "Mobile App",
            Description = "Version 1",
            Status = ProjectStatus.Active
        };
        */

        var project1 = TestDataHelper.CreateProject();
        project1.Name = "Backend API";
        project1.WorkspaceId = workspace.Id;

        var project2 = TestDataHelper.CreateProject();
        project2.Name = "Frontend";
        project2.WorkspaceId = workspace.Id;

        var project3 = TestDataHelper.CreateProject();
        project3.Name = "Mobile App";
        project3.WorkspaceId = workspace.Id;

        _context.Projects.AddRange(project1, project2, project3);

        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllProjectsAsync(user.Id, workspace.Id);

        // Assert
        Assert.Equal(3, result.Count());

        var p1 = result.Any(p => p.Name == project1.Name);
        Assert.True(p1);

        var p2 = result.Any(p => p.Name == project2.Name);
        Assert.True(p2);

        var p3 = result.Any(p => p.Name == project3.Name);
        Assert.True(p3);
    }
}
