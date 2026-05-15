using TechExercise.WebApi.Data;
using TechExercise.WebApi.Models;

namespace TechExercise.WebApi.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TaskRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = "SELECT id, title, description, status, due_date, user_id, created_at, updated_at FROM tasks WHERE id = @id";
        await using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapTask(reader);
        }

        return null;
    }

    public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = "SELECT id, title, description, status, due_date, user_id, created_at, updated_at FROM tasks WHERE user_id = @userId ORDER BY created_at DESC";
        await using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@userId", userId);

        var tasks = new List<TaskItem>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(MapTask(reader));
        }

        return tasks;
    }

    public async Task<int> CreateAsync(TaskItem task)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            INSERT INTO tasks (title, description, status, due_date, user_id, created_at, updated_at)
            VALUES (@title, @description, @status, @due_date, @user_id, @created_at, @updated_at)
            RETURNING id";

        await using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@title", task.Title);
        command.Parameters.AddWithValue("@description", (object?)task.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@status", task.Status);
        command.Parameters.AddWithValue("@due_date", (object?)task.DueDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@user_id", task.UserId);
        command.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
        command.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(TaskItem task)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            UPDATE tasks
            SET title = @title, description = @description, status = @status,
                due_date = @due_date, updated_at = @updated_at
            WHERE id = @id AND user_id = @user_id";

        await using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", task.Id);
        command.Parameters.AddWithValue("@title", task.Title);
        command.Parameters.AddWithValue("@description", (object?)task.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@status", task.Status);
        command.Parameters.AddWithValue("@due_date", (object?)task.DueDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@user_id", task.UserId);
        command.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = "DELETE FROM tasks WHERE id = @id AND user_id = @user_id";
        await using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@user_id", userId);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private static TaskItem MapTask(Npgsql.NpgsqlDataReader reader)
    {
        return new TaskItem
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
            Status = reader.GetString(reader.GetOrdinal("status")),
            DueDate = reader.IsDBNull(reader.GetOrdinal("due_date")) ? null : reader.GetDateTime(reader.GetOrdinal("due_date")),
            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}
