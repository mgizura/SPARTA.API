using SPARTA.Domain.Entities.Provinces;
using SPARTA.Domain.Entities.Provinces.Dtos;
using SPARTA.Domain.Interfaces.Provinces;
using SPARTA.Service.Feature.Provinces.Interfaces;

namespace SPARTA.Service.Feature.Provinces.CaseUse;

public class ProvinceService : IProvinceService
{
    private readonly IProvinceRepository _provinceRepository;

    public ProvinceService(IProvinceRepository provinceRepository)
    {
        _provinceRepository = provinceRepository;
    }

    public async Task<IEnumerable<ProvinceDto>> GetAllProvincesAsync()
    {
        var provinces = await _provinceRepository.GetAllAsync();
        return provinces.Select(p => new ProvinceDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            IsActive = p.IsActive
        });
    }

    public async Task<ProvinceDto?> GetProvinceByIdAsync(int id)
    {
        var province = await _provinceRepository.GetByIdAsync(id);
        if (province == null)
            return null;

        return new ProvinceDto
        {
            Id = province.Id,
            Name = province.Name,
            Code = province.Code,
            IsActive = province.IsActive
        };
    }

    public async Task<ProvinceDto> CreateProvinceAsync(string name, string? code)
    {
        // Validar que el nombre no exista
        if (await _provinceRepository.ExistsByNameAsync(name))
        {
            throw new InvalidOperationException($"Ya existe una provincia con el nombre '{name}'");
        }

        // Validar que el código no exista si se proporciona
        if (!string.IsNullOrWhiteSpace(code) && await _provinceRepository.ExistsByCodeAsync(code))
        {
            throw new InvalidOperationException($"Ya existe una provincia con el código '{code}'");
        }

        var province = new Province
        {
            Name = name,
            Code = code,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdProvince = await _provinceRepository.CreateAsync(province);

        return new ProvinceDto
        {
            Id = createdProvince.Id,
            Name = createdProvince.Name,
            Code = createdProvince.Code,
            IsActive = createdProvince.IsActive
        };
    }
}

