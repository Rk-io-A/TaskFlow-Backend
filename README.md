# TaskFlow Backend

ASP.NET Core 8 Web API — Clean Architecture, JWT + Refresh Tokens, EF Core, PostgreSQL, Redis, SignalR.

## Run

```bash
dotnet restore
dotnet run --project src/TaskFlow.API
```

Swagger: http://localhost:5000/swagger

Default admin: `admin@taskflow.com` / `Admin@123`

## Structure

```
src/
  TaskFlow.API/
  TaskFlow.Application/
  TaskFlow.Domain/
  TaskFlow.Infrastructure/
tests/
  TaskFlow.UnitTests/
```

## Docker

```bash
docker compose up --build
```

## Azure

Deploy `TaskFlow.API` to Azure App Service (.NET 8 Linux).
Set connection strings + JWT in App Settings.
