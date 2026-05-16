# Task Manager - Frontend

Angular frontend application for the Task Management System, communicating with a .NET backend REST API.

## Prerequisites

- Node.js 18+
- Angular CLI 21+
- Backend API running at `http://localhost:50349`

## Setup

1. Install dependencies:
```bash
npm install
```

2. Configure the API URL (optional):
Edit `src/environments/environment.development.ts` to point to your backend API. The default is `http://localhost:50349`.

3. Start the development server:
```bash
npm start
```

The application will be available at `http://localhost:4200`.

## Build

```bash
npm run build
```

Production build output will be in the `dist/` directory.

## Running Tests

```bash
npm test
```

## Project Structure

```
src/
  app/
    core/               # Singleton services, guards, interceptors
      guards/           # Route guards (auth)
      interceptors/     # HTTP interceptors (auth token, error handling)
      services/         # Core services (auth, task API calls)
    features/           # Feature modules
      auth/             # Authentication feature
        components/
          login/        # Login form component
          register/     # Registration form component
        auth.routes.ts  # Auth route configuration
      tasks/            # Task management feature
        components/
          task-list/         # Task list with CRUD operations
          task-form-dialog/  # Create/edit task dialog
        tasks.routes.ts # Tasks route configuration
    layout/             # Layout components
      components/
        navbar/         # Top navigation bar
    models/             # TypeScript interfaces
      auth.models.ts    # Auth request/response types
      task.models.ts    # Task request/response types
    shared/             # Shared components
      components/
        confirm-dialog/ # Reusable confirmation dialog
        empty-state/    # Empty state placeholder component
    app.ts              # Root application component
    app.routes.ts       # Root route configuration
    app.config.ts       # Application providers configuration
  environments/         # Environment configuration
```

## Features

### Authentication
- Login with email and password
- User registration with username, email, and password
- JWT token management with localStorage persistence
- Route protection via auth guard
- Automatic logout on 401 responses

### Task Management
- Create tasks with title, description, status, and due date
- View all tasks in a responsive table (desktop) or card layout (mobile)
- Edit existing tasks via dialog
- Delete tasks with confirmation dialog
- Status chips with color coding (pending, in progress, completed)
- Empty state when no tasks exist
- Snackbar notifications for success/error feedback
- Loading indicators

### Technical Details
- Standalone components (no NgModules)
- Angular Signals for reactive state management
- Reactive Forms with validation
- Angular Material UI components
- Responsive mobile-first design
- Lazy-loaded routes for auth and tasks features
- HTTP interceptors for JWT attachment and error handling
- OnPush change detection ready
- TypeScript strict mode

## API Endpoints

The application integrates with the following backend endpoints:

| Method | Endpoint             | Description        | Auth Required |
|--------|---------------------|--------------------|---------------|
| POST   | /api/Auth/login     | User login         | No            |
| POST   | /api/Auth/register  | User registration  | No            |
| GET    | /api/Tasks          | List all tasks     | Yes           |
| GET    | /api/Tasks/{id}     | Get task by ID     | Yes           |
| POST   | /api/Tasks          | Create a new task  | Yes           |
| PUT    | /api/Tasks/{id}     | Update a task      | Yes           |
| DELETE | /api/Tasks/{id}     | Delete a task      | Yes           |
