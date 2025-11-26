using SPARTA.Domain.Entities.Users.Dtos;

namespace SPARTA.Service.Feature.Users.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(string username, string password);
    Task<string> GenerateTokenAsync(UserDto user);
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

