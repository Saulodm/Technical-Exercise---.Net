using Microsoft.Extensions.Logging;
using TechExercise.Application.DTOs.Tasks;
using TechExercise.Application.Interfaces;
using TechExercise.Domain.Entities;

namespace TechExercise.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<TaskResponse>> GetAllAsync(int userId)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(userId);
        return tasks.Select(MapToResponse);
    }

    public async Task<TaskResponse?> GetByIdAsync(int id, int userId)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null || task.UserId != userId)
            return null;

        return MapToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(int userId, CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            DueDate = request.DueDate,
            UserId = userId
        };

        task.Id = await _taskRepository.CreateAsync(task);
        _logger.LogInformation("Task {TaskId} created for user {UserId}", task.Id, userId);

        return MapToResponse(task);
    }

    public async Task<TaskResponse?> UpdateAsync(int id, int userId, UpdateTaskRequest request)
    {
        var existing = await _taskRepository.GetByIdAsync(id);
        if (existing == null || existing.UserId != userId)
            return null;

        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.Status = request.Status;
        existing.DueDate = request.DueDate;

        var updated = await _taskRepository.UpdateAsync(existing);
        if (!updated)
            return null;

        _logger.LogInformation("Task {TaskId} updated for user {UserId}", id, userId);
        return MapToResponse(existing);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null || task.UserId != userId)
            return false;

        var deleted = await _taskRepository.DeleteAsync(id, userId);
        if (deleted)
            _logger.LogInformation("Task {TaskId} deleted by user {UserId}", id, userId);

        return deleted;
    }

    private static TaskResponse MapToResponse(TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            UserId = task.UserId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
