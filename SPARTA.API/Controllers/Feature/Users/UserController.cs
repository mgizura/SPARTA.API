using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPARTA.API.Controllers.Feature.Models;
using SPARTA.Service.Feature.Users.CaseUse;
using SPARTA.Service.Feature.Users.Interfaces;

namespace SPARTA.API.Controllers.Feature.Users;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IAuthService authService, IUserService userService, ILogger<UserController> logger)
    {
        _authService = authService;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request.Username, request.Password);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var response = new LoginResponse
            {
                Token = result.Token,
                User = new UserResponse
                {
                    Id = result.User.Id,
                    Username = result.User.Username,
                    Email = result.User.Email,
                    FirstName = result.User.FirstName,
                    LastName = result.User.LastName,
                    IsActive = result.User.IsActive
                },
                ExpiresAt = result.ExpiresAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var response = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.IsActive
            });
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    [HttpPut("{id}/change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return BadRequest(new { message = "La contraseña y la confirmación son requeridas" });
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(new { message = "Las contraseñas no coinciden" });
            }

            var result = await _userService.ChangePasswordAsync(id, request.NewPassword);
            if (!result)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Contraseña actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar contraseña");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    [HttpPost("hash-password")]
    [AllowAnonymous]
    public ActionResult<HashPasswordResponse> HashPassword([FromBody] HashPasswordRequest request)
    {
        try
        {
            var hash = AuthService.HashPassword(request.Password);
            return Ok(new HashPasswordResponse { Hash = hash });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar hash de contraseña");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}

public class ChangePasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class HashPasswordRequest
{
    public string Password { get; set; } = string.Empty;
}

public class HashPasswordResponse
{
    public string Hash { get; set; } = string.Empty;
}
