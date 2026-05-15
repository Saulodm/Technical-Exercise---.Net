using TechExercise.WebApi.Models;

namespace TechExercise.WebApi.Auth
{
    public interface IJwtService
    {
        string GenerateToken(User user);

    }
}
