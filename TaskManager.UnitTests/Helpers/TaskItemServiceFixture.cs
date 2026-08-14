using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Services;
using TaskManager.UnitTests.helpers;

namespace TaskManager.UnitTests.Helpers;

public class TaskItemServiceFixture
{
    public AppDbContext Context { get; }
    public TaskItemService Sut { get; }

    public AppUser User { get; }
    public Workspace Workspace { get; }
    public WorkspaceMember Member { get; }
    public Project Project { get; }

    public TaskItemServiceFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new AppDbContext(options);
        Sut = new TaskItemService(Context);

        User = TestDataHelper.CreateUser();
        Context.Users.Add(User);

        Workspace = TestDataHelper.CreateWorkspace();
        Context.Workspaces.Add(Workspace);

        Member = new WorkspaceMember
        {
            UserId = User.Id,
            WorkspaceId = Workspace.Id,
            Role = Core.Enums.WorkspaceRole.Manager
        };
        Context.WorkspaceMembers.Add(Member);

        Project = TestDataHelper.CreateProject();
        Project.WorkspaceId = Workspace.Id;
        Context.Projects.Add(Project);

        Context.SaveChanges();
    }

}
