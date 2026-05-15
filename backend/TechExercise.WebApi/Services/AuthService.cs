using System.ComponentModel.DataAnnotations;
using TechExercise.WebApi.Auth;
using TechExercise.WebApi.DTOs.Auth;
using TechExercise.WebApi.Exceptions;
using TechExercise.WebApi.Models;
using TechExercise.WebApi.Repositories;

namespace TechExercise.WebApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ValidationException("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required.");

        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new ConflictException("Email already registered.");

        var user = new User
        {
            Name = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        user.Id = await _userRepository.CreateAsync(user);
        _logger.LogInformation("User {Email} registered successfully", user.Email);
        var token = _jwtService.GenerateToken(user);
        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            IdUser = user.Id,
            Username = user.Name
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required.");

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");
        var token = _jwtService.GenerateToken(user);
        _logger.LogInformation("User {Email} logged in successfully", user.Email);
        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            IdUser = user.Id,
            Username = user.Name
        };
    }

}
