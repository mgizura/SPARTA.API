using SPARTA.Domain.Entities.Provinces.Dtos;

namespace SPARTA.Service.Feature.Provinces.Interfaces;

public interface IProvinceService
{
    Task<IEnumerable<ProvinceDto>> GetAllProvincesAsync();
    Task<ProvinceDto?> GetProvinceByIdAsync(int id);
    Task<ProvinceDto> CreateProvinceAsync(string name, string? code);
}

