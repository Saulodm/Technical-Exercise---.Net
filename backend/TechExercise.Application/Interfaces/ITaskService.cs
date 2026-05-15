using TechExercise.Application.DTOs.Tasks;

namespace TechExercise.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetAllAsync(int userId);
    Task<TaskResponse?> GetByIdAsync(int id, int userId);
    Task<TaskResponse> CreateAsync(int userId, CreateTaskRequest request);
    Task<TaskResponse?> UpdateAsync(int id, int userId, UpdateTaskRequest request);
    Task<bool> DeleteAsync(int id, int userId);
}
