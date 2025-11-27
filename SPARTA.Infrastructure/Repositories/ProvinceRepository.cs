using Microsoft.EntityFrameworkCore;
using SPARTA.Domain.Entities.Provinces;
using SPARTA.Domain.Interfaces.Provinces;

namespace SPARTA.Infrastructure.Repositories;

public class ProvinceRepository : IProvinceRepository
{
    private readonly ApplicationDbContext _context;

    public ProvinceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Province?> GetByIdAsync(int id)
    {
        return await _context.Provinces
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Province>> GetAllAsync()
    {
        return await _context.Provinces
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Province> CreateAsync(Province province)
    {
        _context.Provinces.Add(province);
        await _context.SaveChangesAsync();
        return province;
    }

    public async Task<Province> UpdateAsync(Province province)
    {
        province.UpdatedAt = DateTime.UtcNow;
        _context.Provinces.Update(province);
        await _context.SaveChangesAsync();
        return province;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Provinces
            .AnyAsync(p => p.Name == name);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        return await _context.Provinces
            .AnyAsync(p => p.Code == code);
    }
}

