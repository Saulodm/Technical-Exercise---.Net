using TechExercise.Domain.Entities;

namespace TechExercise.Application.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId);
    Task<int> CreateAsync(TaskItem task);
    Task<bool> UpdateAsync(TaskItem task);
    Task<bool> DeleteAsync(int id, int userId);
}
