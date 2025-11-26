using SPARTA.Domain.Entities.Users;
using SPARTA.Domain.Entities.Users.Dtos;
using SPARTA.Domain.Interfaces.Users;
using SPARTA.Service.Feature.Users.CaseUse;
using SPARTA.Service.Feature.Users.Interfaces;

namespace SPARTA.Service.Feature.Users.CaseUse;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive
        });
    }

    public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        user.PasswordHash = AuthService.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }
}

