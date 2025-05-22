using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientController: ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientController> _logger;

    public PatientController(
        IPatientService patientService,
        ILogger<PatientController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPatientDetails(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("ID pacjenta musi być liczbą większą od zera");
            }

            var patientDetails = await _patientService.GetPatientDetailsAsync(id);

            return Ok(patientDetails);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, $"Nie znaleziono pacjenta o ID {id}");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Nieoczekiwany błąd podczas pobierania danych pacjenta o ID {id}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Wystąpił błąd podczas przetwarzania żądania");
        }
    }
}