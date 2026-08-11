using TaskManager.Core.Entities;

namespace TaskManager.UnitTests.helpers;

public class TestDataHelper
{
    public static AppUser CreateUser()
    {
        return new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            FullName = "Test User",
            Email = "Test@TaskFlow.com",
            UserName = "Test@TaskFlow.com"
        };
    }

    public static Workspace CreateWorkspace()
    {
        return new Workspace
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid().ToString(),
            Name = "Test Workspace"
        };
    }

    public static Project CreateProject()
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            WorkspaceId = Guid.NewGuid(),
            Name = "Test Project",
            Description = "Version 1"
        };
    }
}