using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Core.Enums;
using TaskManager.Infrastructure.Services;
using TaskManager.Core.Exceptions;
namespace TaskManager.UnitTests;

public class WorkspaceMemberServiceTests
{
    [Fact]
    public async Task RemoveMemberAsync_ManagerTargetsAnotherManager_Throws()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var workspaceId = Guid.NewGuid();
        var requesterId = Guid.NewGuid().ToString();
        var targetUserId = Guid.NewGuid().ToString();
        var workspace = new Workspace { OwnerId = Guid.NewGuid().ToString(), Id = workspaceId, Name = "Test Workspace" };

        var requester = new AppUser { Id = requesterId, UserName = "requester@TaskFlow.com", FullName = "requester", Email = "requester@TaskFlow.com" };
        var target = new AppUser { Id = targetUserId, UserName = "target@TaskFlow.com", FullName = "target", Email = "target@TaskFlow.com" };

        context.AddRange(requester, target);

        context.Workspaces.Add(workspace);
        context.WorkspaceMembers.Add(new WorkspaceMember { Workspace = workspace, UserId = requesterId, Role = WorkspaceRole.Manager });
        context.WorkspaceMembers.Add(new WorkspaceMember { Workspace = workspace, UserId = targetUserId, Role = WorkspaceRole.Manager });
        await context.SaveChangesAsync();

        var sut = new WorkspaceMemberService(context, null!);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<AppException>(()
            => sut.RemoveMemberAsync(requesterId, workspaceId, targetUserId));

        Assert.Equal("Can't remove this user", ex.Message);
    }

    [Fact]
    public async Task RemoveMemberAsync_OwnerTargetsManager_Succeed()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        var workspaceId = Guid.NewGuid();
        var requesterId = Guid.NewGuid().ToString();
        var targetUserId = Guid.NewGuid().ToString();
        var workspace = new Workspace { Id = workspaceId, OwnerId = requesterId, Name = "Test Workspace" };

        var requesterUser = new AppUser { Id = requesterId, UserName = "requester@test.com", Email = "requester@test.com", FullName = "Requester" };
        var targetUser = new AppUser { Id = targetUserId, UserName = "target@test.com", Email = "target@test.com", FullName = "Target" };
        context.Users.AddRange(requesterUser, targetUser);

        context.Workspaces.Add(workspace);
        context.WorkspaceMembers.Add(new WorkspaceMember { Workspace = workspace, UserId = requesterId, Role = WorkspaceRole.Owner });
        context.WorkspaceMembers.Add(new WorkspaceMember { Workspace = workspace, UserId = targetUserId, Role = WorkspaceRole.Manager });
        await context.SaveChangesAsync();

        var sut = new WorkspaceMemberService(context, null!);

        // Act + Assert
        await sut.RemoveMemberAsync(requesterId, workspaceId, targetUserId);

        var stillExists = await context.WorkspaceMembers
            .AnyAsync(wm => wm.UserId == targetUserId && wm.WorkspaceId == workspaceId);
        Assert.False(stillExists);
    }

    [Fact]
    public async Task RemoveMemberAsync_ManagerTargetsOwner_Throws()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

        var context = new AppDbContext(options);

        var workspaceId = Guid.NewGuid();
        var requesterId = Guid.NewGuid().ToString();
        var targetUserId = Guid.NewGuid().ToString();

        var workspace = new Workspace { Id = workspaceId, OwnerId = targetUserId, Name = "Test Workspace" };
        context.Workspaces.Add(workspace);

        var requester = new AppUser { Id = requesterId, UserName = "requester@TaskFlow.com", FullName = "requester", Email = "requester@TaskFlow.com" };
        var target = new AppUser { Id = targetUserId, UserName = "target@TaskFlow.com", FullName = "target", Email = "target@TaskFlow.com" };
        context.Users.AddRange(requester, target);

        context.WorkspaceMembers.Add(new WorkspaceMember { UserId = requesterId, Workspace = workspace, Role = WorkspaceRole.Manager });
        context.WorkspaceMembers.Add(new WorkspaceMember { UserId = targetUserId, Workspace = workspace, Role = WorkspaceRole.Owner });
        await context.SaveChangesAsync();

        var sut = new WorkspaceMemberService(context, null!);

        // Act + Assert
        var ex = await Assert.ThrowsAsync<AppException>(()
            => sut.RemoveMemberAsync(requesterId, workspaceId, targetUserId));

        Assert.Equal("Can't remove this user", ex.Message);
    }
}