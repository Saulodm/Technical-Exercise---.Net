# TechExercise — Task Management System

A full-stack **Task Management System** with a **.NET 10** backend REST API and an **Angular 21** frontend, using **PostgreSQL** for data persistence and **JWT** for authentication.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (Angular 21)                     │
│              http://localhost:4200                           │
│                                                             │
│  Login / Register  ───►  Task CRUD  ◄──►  Auth Guard       │
│       ▲                         ▲                           │
│       │       HTTP (CORS)       │                           │
│       ▼                         ▼                           │
│           http://localhost:50349/api/*                       │
├─────────────────────────────────────────────────────────────┤
│                    Backend (.NET 10 API)                     │
│              http://localhost:50349                          │
│                                                             │
│  AuthController  ──  TaskController  ──  JWT Validation     │
│       │                  │                                   │
│       ▼                  ▼                                   │
│           PostgreSQL (port 5432)                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Tech Stack

| Layer       | Technology                                               |
|-------------|----------------------------------------------------------|
| Frontend    | Angular 21, TypeScript strict, Angular Material, SCSS    |
| Backend     | .NET 10, ASP.NET Core, Clean Architecture                |
| Database    | PostgreSQL 16 (via Docker or native)                     |
| Auth        | JWT (Bearer tokens) + BCrypt password hashing            |
| Tests       | Vitest (frontend), xUnit + Testcontainers (backend)      |

---

## Quick Start

### Prerequisites

- **Backend**: [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- **Frontend**: [Node.js 18+](https://nodejs.org/), Angular CLI 21+ (`npm install -g @angular/cli`)

### 1. Start the Database

```bash
cd backend
docker compose up -d
```

### 2. Run the Backend API

```bash
cd backend
dotnet run --project TechExercise.WebApi
```

The API starts at `http://localhost:50349`. Swagger UI at `http://localhost:50349/Swagger`.

### 3. Run the Frontend

```bash
cd frontend
npm install
npm start
```

The application opens at `http://localhost:4200`.

---

## Demo Credentials

The database is pre-seeded with demo data on first initialization:

| Field      | Value                  |
|------------|------------------------|
| Username   | `admin`                |
| Email      | `admin@example.com`    |
| Password   | `password123`          |

8 sample tasks are also seeded across different statuses (completed, in_progress, pending).

---

## API Endpoints

| Method | Endpoint             | Auth Required | Description              |
|--------|----------------------|---------------|--------------------------|
| POST   | `/api/Auth/register` | No            | Create a new user        |
| POST   | `/api/Auth/login`    | No            | Login, returns JWT       |
| GET    | `/api/Tasks`         | Yes           | List user's tasks        |
| GET    | `/api/Tasks/{id}`    | Yes           | Get task by ID           |
| POST   | `/api/Tasks`         | Yes           | Create a task            |
| PUT    | `/api/Tasks/{id}`    | Yes           | Update a task            |
| DELETE | `/api/Tasks/{id}`    | Yes           | Delete a task            |

Task status values: `pending`, `in_progress`, `completed`

---

## Project Structure

```
TechExercise---.Net/
│
├── backend/                              # .NET 10 API
│   ├── TechExercise.Domain/              #   Domain entities
│   ├── TechExercise.Application/         #   Business logic, interfaces, DTOs
│   ├── TechExercise.Infrastructure/      #   Database, JWT, BCrypt implementations
│   ├── TechExercise.WebApi/              #   Controllers, middleware, DI
│   ├── TechExercise.Tests/               #   Unit & integration tests
│   ├── database/                         #   SQL init scripts
│   ├── docker-compose.yml                #   PostgreSQL container
│   └── README.md                         #   Backend-specific documentation
│
├── frontend/                             # Angular 21 SPA
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/                     #   Services, guards, interceptors
│   │   │   ├── features/auth/            #   Login & register
│   │   │   ├── features/tasks/           #   Task CRUD
│   │   │   ├── layout/                   #   Navbar
│   │   │   ├── shared/                   #   Shared components
│   │   │   └── models/                   #   TypeScript interfaces
│   │   └── environments/                 #   API URL config
│   └── README.md                         #   Frontend-specific documentation
│
└── README.md                             # This file
```

---

## Running Tests

### Backend Tests

```bash
cd backend
dotnet test TechExercise.Tests
```

Integration tests use **Testcontainers** (requires Docker).

### Frontend Tests

```bash
cd frontend
npm test
```

---

## Key Design Decisions

- **Clean Architecture** on the backend with strict one-way dependency rule
- **Pure ADO.NET** for database access — no ORM, no EF Core
- **Standalone components** on the frontend — no NgModules
- **Angular Signals** for reactive state management — no NgRx
- **JWT stateless authentication** with BCrypt password hashing
- **No overengineering** — no CQRS, MediatR, microservices, or event sourcing
