# TaskFlow API

A multi-tenant team collaboration REST API for managing workspaces, projects, tasks, and team members — built with ASP.NET Core and Clean Architecture.

TaskFlow lets teams of any size — from a group of friends to a company department — organize their work inside shared workspaces, track project progress, manage tasks with priorities and deadlines, and communicate through comments.

---

## Tech Stack

- **Framework:** ASP.NET Core (.NET 10)
- **Architecture:** Clean Architecture (Core / Infrastructure / API)
- **Database:** SQL Server with Entity Framework Core 10
- **Authentication:** ASP.NET Core Identity + JWT Bearer tokens
- **API Docs:** Swagger UI (Swashbuckle)

---

## Architecture

The solution is split into three projects following Clean Architecture principles:

- **TaskManager.Core** — entities, interfaces, DTOs, enums, and exceptions. No external dependencies.
- **TaskManager.Infrastructure** — EF Core DbContext, entity configurations, migrations, and service implementations.
- **TaskManager.API** — controllers, middleware, and the application entry point.

Dependencies only flow inward: API → Infrastructure → Core. The Core layer knows nothing about HTTP or databases.

---

## Features

- **JWT Authentication** — register and login with secure token-based authentication
- **Role-Based Authorization** — three workspace roles (Owner, Manager, Member) with different permissions enforced at the service layer
- **Workspace Management** — create workspaces, invite members, promote/demote roles, remove members
- **Project Management** — organize work into projects scoped to a workspace
- **Task Management** — create tasks with title, description, priority, deadline, and assignee; any member can update task status independently
- **Comments** — comment on any task; authors and moderators can delete comments
- **Pagination & Filtering** — filter tasks by status, priority, assignee, and deadline with page-based pagination

---

## Role Permissions

| Action                            | Owner | Manager | Member |
| --------------------------------- | :---: | :-----: | :----: |
| Delete workspace                  |  ✅   |   ❌    |   ❌   |
| Invite members                    |  ✅   |   ✅    |   ❌   |
| Promote to Manager                |  ✅   |   ❌    |   ❌   |
| Remove members                    |  ✅   |  ✅\*   |   ❌   |
| Create / update / delete projects |  ✅   |   ✅    |   ❌   |
| Create / update / delete tasks    |  ✅   |   ✅    |   ❌   |
| Update task status                |  ✅   |   ✅    |   ✅   |
| Comment on tasks                  |  ✅   |   ✅    |   ✅   |
| Delete own comment                |  ✅   |   ✅    |   ✅   |
| Delete any comment                |  ✅   |   ✅    |   ❌   |

\*Managers can only remove Members, not other Managers or the Owner.

---

## API Endpoints

### Auth

| Method | Endpoint             | Description                 |
| ------ | -------------------- | --------------------------- |
| POST   | `/api/auth/register` | Register a new user         |
| POST   | `/api/auth/login`    | Login and receive JWT token |

### Workspaces

| Method | Endpoint              | Description                         |
| ------ | --------------------- | ----------------------------------- |
| GET    | `/api/workspace`      | Get all workspaces for current user |
| GET    | `/api/workspace/{id}` | Get workspace by ID                 |
| POST   | `/api/workspace`      | Create a new workspace              |
| PUT    | `/api/workspace/{id}` | Update workspace                    |
| DELETE | `/api/workspace/{id}` | Delete workspace (Owner only)       |

### Workspace Members

| Method | Endpoint                                                | Description               |
| ------ | ------------------------------------------------------- | ------------------------- |
| GET    | `/api/workspace/{workspaceId}/members`                  | List all members          |
| POST   | `/api/workspace/{workspaceId}/members`                  | Invite a member by email  |
| PUT    | `/api/workspace/{workspaceId}/members/{userId}/promote` | Promote member to Manager |
| DELETE | `/api/workspace/{workspaceId}/members/{userId}`         | Remove a member           |

### Projects

| Method | Endpoint                                            | Description       |
| ------ | --------------------------------------------------- | ----------------- |
| GET    | `/api/workspace/{workspaceId}/projects`             | Get all projects  |
| GET    | `/api/workspace/{workspaceId}/projects/{projectId}` | Get project by ID |
| POST   | `/api/workspace/{workspaceId}/projects`             | Create a project  |
| PUT    | `/api/workspace/{workspaceId}/projects/{projectId}` | Update a project  |
| DELETE | `/api/workspace/{workspaceId}/projects/{projectId}` | Delete a project  |

### Tasks

| Method | Endpoint                                                                  | Description                             |
| ------ | ------------------------------------------------------------------------- | --------------------------------------- |
| GET    | `/api/workspace/{workspaceId}/projects/{projectId}/tasks`                 | Get tasks (with filtering & pagination) |
| GET    | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}`        | Get task by ID                          |
| POST   | `/api/workspace/{workspaceId}/projects/{projectId}/tasks`                 | Create a task                           |
| PUT    | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}`        | Update a task                           |
| PUT    | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}/status` | Update task status only                 |
| DELETE | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}`        | Delete a task                           |

#### Task Filtering Query Parameters

| Parameter        | Type         | Example                      |
| ---------------- | ------------ | ---------------------------- |
| `status`         | enum         | `?status=InProgress`         |
| `priority`       | enum         | `?priority=High`             |
| `assignedToId`   | string       | `?assignedToId=userId`       |
| `deadlineBefore` | datetime     | `?deadlineBefore=2026-08-01` |
| `page`           | int          | `?page=2`                    |
| `pageSize`       | int (max 50) | `?pageSize=10`               |

### Comments

| Method | Endpoint                                                                                | Description                  |
| ------ | --------------------------------------------------------------------------------------- | ---------------------------- |
| GET    | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}/comments`             | Get all comments             |
| GET    | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}/comments/{commentId}` | Get comment by ID            |
| POST   | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}/comments`             | Add a comment                |
| PUT    | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}/comments/{commentId}` | Edit a comment (author only) |
| DELETE | `/api/workspace/{workspaceId}/projects/{projectId}/tasks/{taskId}/comments/{commentId}` | Delete a comment             |

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (local or remote)

### Setup

1. **Clone the repository**

```bash
git clone https://github.com/zezoz5/TaskFlow.git
cd TaskFlow
```

2. **Create your local settings file**

Create `appsettings.Development.json` inside `TaskManager.API/` — this file is git-ignored and must never be committed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TaskFlowDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "TaskFlowAPI",
    "Audience": "TaskFlowClient",
    "ExpireDays": 7
  }
}
```

3. **Apply migrations**

```bash
cd TaskManager.API
dotnet ef database update
```

4. **Run the API**

```bash
dotnet run --launch-profile https
```

5. **Open Swagger UI**

Navigate to `https://localhost:7173/swagger` to explore and test all endpoints.

To authenticate in Swagger: call `/api/auth/login`, copy the token from the response, click the **Authorize** button, and paste the token.

---

## What I Learned

- Implementing **Clean Architecture** in a real project — keeping domain logic completely independent of HTTP and database concerns
- **JWT authentication** from scratch — token generation, claim mapping, and the `MapInboundClaims = false` gotcha
- **Role-based authorization** enforced at the service layer, not just at the controller level
- **EF Core navigation properties** — when `.Select()` auto-joins vs. when `.Include()` is required
- **Guard clause pattern** for cleaner, more readable authorization logic
- **Pagination and filtering** using incremental LINQ query building
- Real debugging skills — tracing 401 errors through middleware, claim inspection, package version conflicts
