-- TechExercise Task Management System - Database Initialization
-- Run this script against a PostgreSQL database to create all required tables.

CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(500) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS tasks (
    id SERIAL PRIMARY KEY,
    title VARCHAR(300) NOT NULL,
    description TEXT,
    status VARCHAR(20) NOT NULL DEFAULT 'pending',
    due_date DATE,
    user_id INTEGER NOT NULL REFERENCES users(id),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_tasks_user_id ON tasks(user_id);
CREATE INDEX IF NOT EXISTS idx_tasks_status ON tasks(status);

-- =============================================
-- Seeded data / demo credentials
-- =============================================

INSERT INTO users (username, email, password_hash)
VALUES
    ('admin', 'admin@example.com', '$2b$10$hD84DP1Ax6/ClYXQmCs6b.YVMX7wjGm4Jew7YHHgyB5i0tRGIeM.2')
ON CONFLICT (username) DO NOTHING;

INSERT INTO tasks (title, description, status, due_date, user_id)
VALUES
    ('Set up development environment', 'Install and configure all necessary tools and dependencies for the project.', 'completed', CURRENT_DATE - 5, 1),
    ('Design database schema', 'Create the database schema with tables for users and tasks.', 'completed', CURRENT_DATE - 3, 1),
    ('Implement user authentication', 'Build login and registration functionality with JWT tokens.', 'in_progress', CURRENT_DATE + 2, 1),
    ('Create task CRUD endpoints', 'Implement the REST API endpoints for creating, reading, updating, and deleting tasks.', 'in_progress', CURRENT_DATE + 7, 1),
    ('Write unit tests', 'Add comprehensive unit tests for services, repositories, and controllers.', 'pending', CURRENT_DATE + 14, 1),
    ('Add input validation', 'Implement request validation for all API endpoints using FluentValidation or data annotations.', 'pending', CURRENT_DATE + 10, 1),
    ('Setup CI/CD pipeline', 'Configure continuous integration and deployment workflows.', 'pending', CURRENT_DATE + 21, 1),
    ('Create API documentation', 'Document all endpoints with request/response examples using Swagger.', 'pending', CURRENT_DATE + 28, 1);
