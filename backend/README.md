# TechExercise -- Task Management API

A production-ready **Task Management REST API** built with **.NET 10**, **ASP.NET Core**, **PostgreSQL** (pure ADO.NET), and **JWT Authentication**.

---

## Backend Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for database)
- [PostgreSQL 16](https://www.postgresql.org/download/) (alternative if not using Docker)

### 1. Clone & Restore

```bash
cd backend
dotnet restore
```

### 2. Configure Secrets

The development configuration is in `TechExercise.WebApi/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=techexercise;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "ThisIsASecureKeyForDevelopment_TechExercise_2026",
    "Issuer": "TechExercise",
    "Audience": "TechExercise",
    "ExpirationInHours": 8
  }
}
```

For production, override these values via environment variables or user secrets.

### 3. Run the API

```bash
dotnet run --project TechExercise.WebApi
```

The API starts on `https://localhost:50348` (or the next available port).

Swagger UI is available at `https://localhost:50348/Swagger`.

---

## Database Setup

### Option A: Docker (Recommended)

```bash
cd backend
docker compose up -d
```

This starts a PostgreSQL 16 Alpine container with:
- Database: `techexercise`
- User: `postgres`
- Password: `postgres`
- Port: `5432`
- Auto-runs `database/init.sql` to create tables on first start

### Option B: Manual PostgreSQL

Run the SQL script directly:

```bash
psql -U postgres -d techexercise -f database/init.sql
```

### Database Schema

```sql
users
  id          SERIAL PRIMARY KEY
  username    VARCHAR(100) UNIQUE NOT NULL
  email       VARCHAR(255) UNIQUE NOT NULL
  password_hash VARCHAR(500) NOT NULL
  created_at  TIMESTAMPTZ DEFAULT NOW()
  updated_at  TIMESTAMPTZ DEFAULT NOW()

tasks
  id          SERIAL PRIMARY KEY
  title       VARCHAR(300) NOT NULL
  description TEXT
  status      VARCHAR(20) DEFAULT 'pending'
  due_date    DATE
  user_id     INTEGER NOT NULL REFERENCES users(id)
  created_at  TIMESTAMPTZ DEFAULT NOW()
  updated_at  TIMESTAMPTZ DEFAULT NOW()
```

---

## Commands

### Run the API

```bash
dotnet run --project TechExercise.WebApi
```

### Run All Tests

```bash
dotnet test TechExercise.Tests
```

### Run Only Unit Tests (skip integration tests)

```bash
dotnet test TechExercise.Tests --filter "FullyQualifiedName!~Repositories"
```

### Run Only Integration Tests

```bash
dotnet test TechExercise.Tests --filter "FullyQualifiedName~Repositories"
```

Integration tests use **Testcontainers** to spin up a disposable PostgreSQL container. Docker must be running.

### Build

```bash
dotnet build
```

---

## Demo Credentials

After starting the API, register a user via the endpoint:

```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "demouser",
    "email": "demo@example.com",
    "password": "password123"
  }'
```

Then login to obtain a JWT:

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "demo@example.com",
    "password": "password123"
  }'
```

Response:

```json
{
  "idUser": 1,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "username": "demouser",
  "email": "demo@example.com"
}
```

Use the token as `Bearer {token}` in subsequent requests.

---

## API Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/api/auth/register` | Public | Register a new user |
| POST | `/api/auth/login` | Public | Login, returns JWT |
| GET | `/api/tasks` | JWT | List authenticated user's tasks |
| GET | `/api/tasks/{id}` | JWT | Get a specific task |
| POST | `/api/tasks` | JWT | Create a new task |
| PUT | `/api/tasks/{id}` | JWT | Update a task |
| DELETE | `/api/tasks/{id}` | JWT | Delete a task |

### Task Status Values

- `pending`
- `in_progress`
- `completed`

### Task Model (JSON)

```json
{
  "title": "Implement login",
  "description": "Add JWT authentication",
  "status": "in_progress",
  "dueDate": "2026-06-01"
}
```

---

## Project Structure

```
backend/
├── TechExercise.slnx                          # Solution file
├── docker-compose.yml                         # PostgreSQL container
├── database/
│   └── init.sql                               # Database initialization script
├── TechExercise.WebApi/                       # Main API project
│   ├── Program.cs                             # Host configuration & DI
│   ├── appsettings.json                       # Base configuration
│   ├── appsettings.Development.json            # Development overrides
│   ├── TechExercise.WebApi.csproj             # Project file
│   ├── Auth/
│   │   ├── IJwtService.cs                     # JWT generation contract
│   │   └── JwtService.cs                      # JWT token generation
│   ├── Controllers/
│   │   ├── AuthController.cs                  # Register / Login endpoints
│   │   └── TasksController.cs                 # CRUD endpoints
│   ├── DTOs/
│   │   ├── Auth/
│   │   │   ├── RegisterRequest.cs
│   │   │   ├── LoginRequest.cs
│   │   │   └── LoginResponse.cs
│   │   └── Tasks/
│   │       ├── CreateTaskRequest.cs
│   │       ├── UpdateTaskRequest.cs
│   │       └── TaskResponse.cs
│   ├── Data/
│   │   └── DbConnectionFactory.cs             # Npgsql connection factory
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs      # Global error handling
│   ├── Models/
│   │   ├── User.cs
│   │   └── TaskItem.cs
│   ├── Repositories/
│   │   ├── IUserRepository.cs / UserRepository.cs
│   │   └── ITaskRepository.cs / TaskRepository.cs
│   └── Services/
│       ├── IAuthService.cs / AuthService.cs
│       └── ITaskService.cs / TaskService.cs
└── TechExercise.Tests/                        # Test project
    ├── TechExercise.Tests.csproj
    ├── Controllers/
    │   ├── AuthControllerTests.cs
    │   └── TasksControllerTests.cs
    ├── Services/
    │   ├── AuthServiceTests.cs
    │   └── TaskServiceTests.cs
    ├── Middleware/
    │   └── ExceptionHandlingMiddlewareTests.cs
    ├── Validation/
    │   └── DtoValidationTests.cs
    └── Repositories/
        ├── DatabaseFixture.cs                  # Testcontainers PostgreSQL lifecycle
        ├── UserRepositoryTests.cs
        └── TaskRepositoryTests.cs
```

---

## Architecture Principles

- **Simple layered**: Controller -> Service -> Repository -> Database
- **Pure ADO.NET**: No EF Core, no Dapper, no ORM abstractions
- **Manual DTO mapping**: Explicit, debuggable, no AutoMapper
- **JWT Bearer auth**: BCrypt password hashing, stateless tokens
- **Global error handling**: Middleware catches and normalizes all exceptions
- **Input validation**: Data Annotations on request DTOs
- **User-scoped tasks**: Each user only sees and manages their own tasks
- **No overengineering**: No CQRS, no MediatR, no event sourcing, no microservices
