using SPARTA.Domain.Entities.Provinces;

namespace SPARTA.Domain.Interfaces.Provinces;

public interface IProvinceRepository
{
    Task<Province?> GetByIdAsync(int id);
    Task<IEnumerable<Province>> GetAllAsync();
    Task<Province> CreateAsync(Province province);
    Task<Province> UpdateAsync(Province province);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByCodeAsync(string code);
}

