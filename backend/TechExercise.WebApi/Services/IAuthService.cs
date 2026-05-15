using TechExercise.WebApi.DTOs.Auth;

namespace TechExercise.WebApi.Services;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
