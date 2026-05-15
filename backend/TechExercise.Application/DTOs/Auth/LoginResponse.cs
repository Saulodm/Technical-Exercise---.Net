namespace TechExercise.Application.DTOs.Auth;

public class LoginResponse
{
    public int IdUser { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
