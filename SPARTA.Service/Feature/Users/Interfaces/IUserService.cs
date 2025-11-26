using SPARTA.Domain.Entities.Users.Dtos;

namespace SPARTA.Service.Feature.Users.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<bool> ChangePasswordAsync(int userId, string newPassword);
}

