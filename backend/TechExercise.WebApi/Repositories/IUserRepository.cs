using TechExercise.WebApi.Models;

namespace TechExercise.WebApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<int> CreateAsync(User user);
}
