using TechExercise.Domain.Entities;

namespace TechExercise.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
