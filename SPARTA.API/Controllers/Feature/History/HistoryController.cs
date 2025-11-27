using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPARTA.API.Controllers.Feature.Models;
using SPARTA.Service.Feature.History.Interfaces;

namespace SPARTA.API.Controllers.Feature.History;

[ApiController]
[Route("api/[controller]")]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;
    private readonly ILogger<HistoryController> _logger;

    public HistoryController(IHistoryService historyService, ILogger<HistoryController> logger)
    {
        _historyService = historyService;
        _logger = logger;
    }

    [HttpPost("generate")]
    [Authorize]
    public async Task<ActionResult<string>> Generate([FromBody] GenerateHistoryRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { message = "El texto no puede estar vacío" });
            }

            var result = await _historyService.GenerateAsync(request.Text, request.HasTest, request.HasUpgrade);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar historial");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}

