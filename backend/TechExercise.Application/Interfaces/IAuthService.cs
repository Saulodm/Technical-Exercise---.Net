using TechExercise.Application.DTOs.Auth;

namespace TechExercise.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
