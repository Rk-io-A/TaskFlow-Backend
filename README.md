# TaskFlow Backend

ASP.NET Core 8 portfolio project demonstrating Clean Architecture, JWT + refresh tokens, EF Core, PostgreSQL, Redis and SignalR-ready application patterns.

> **Status:** engineering demo / portfolio project. It is not a hosted production service and contains no built-in production administrator credential.

## Structure

```text
src/
├── TaskFlow.API/
├── TaskFlow.Application/
├── TaskFlow.Domain/
└── TaskFlow.Infrastructure/

tests/
└── TaskFlow.UnitTests/
```

## Local configuration

Copy the environment template and replace the placeholders:

```bash
cp .env.example .env
```

At minimum configure a strong PostgreSQL password and a random JWT signing key of at least 32 characters.

The optional demo administrator is created **only in Development** and only when both of these values are explicitly configured:

```text
DemoAdmin__Email
DemoAdmin__Password
```

No default admin password is compiled into the application.

## Run with .NET

```bash
dotnet restore

dotnet run --project src/TaskFlow.API
```

Swagger is available only in the Development environment.

## Run with Docker Compose

Create `.env` first, then:

```bash
docker compose up --build
```

Docker Compose requires:

```text
TASKFLOW_POSTGRES_PASSWORD
TASKFLOW_JWT_KEY
```

Optional local demo administrator:

```text
TASKFLOW_DEMO_ADMIN_EMAIL
TASKFLOW_DEMO_ADMIN_PASSWORD
```

## Deployment notes

The API can be adapted for Azure App Service or another container platform, but a real deployment must provide secrets through the platform secret/configuration store rather than committing them to the repository.

Before any public deployment:

- use a managed PostgreSQL database;
- use managed Redis or disable it if unused;
- configure strict CORS origins;
- configure a unique JWT signing key through secrets;
- disable demo-admin seeding;
- run database migrations intentionally;
- enable HTTPS, logging and monitoring.

## Security

- Never commit `.env`.
- Never reuse the example placeholders as production credentials.
- Do not expose database ports publicly in production.
- Keep Swagger disabled in production unless intentionally required.
- Rotate any credential that has previously been used outside local development.
