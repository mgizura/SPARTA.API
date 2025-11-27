using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPARTA.Service.Feature.Provinces.Interfaces;

namespace SPARTA.API.Controllers.Feature.Provinces;

[ApiController]
[Route("api/[controller]")]
public class ProvinceController : ControllerBase
{
    private readonly IProvinceService _provinceService;
    private readonly ILogger<ProvinceController> _logger;

    public ProvinceController(IProvinceService provinceService, ILogger<ProvinceController> logger)
    {
        _provinceService = provinceService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ProvinceResponse>>> GetAllProvinces()
    {
        try
        {
            var provinces = await _provinceService.GetAllProvincesAsync();
            var response = provinces.Select(p => new ProvinceResponse
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                IsActive = p.IsActive
            });
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener provincias");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ProvinceResponse>> GetProvinceById(int id)
    {
        try
        {
            var province = await _provinceService.GetProvinceByIdAsync(id);
            if (province == null)
            {
                return NotFound(new { message = "Provincia no encontrada" });
            }

            var response = new ProvinceResponse
            {
                Id = province.Id,
                Name = province.Name,
                Code = province.Code,
                IsActive = province.IsActive
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener provincia");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProvinceResponse>> CreateProvince([FromBody] CreateProvinceRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "El nombre de la provincia es requerido" });
            }

            var province = await _provinceService.CreateProvinceAsync(request.Name, request.Code);
            var response = new ProvinceResponse
            {
                Id = province.Id,
                Name = province.Name,
                Code = province.Code,
                IsActive = province.IsActive
            };

            return CreatedAtAction(nameof(GetProvinceById), new { id = province.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Intento de crear provincia duplicada");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear provincia");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}

public class CreateProvinceRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
}

public class ProvinceResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; }
}

