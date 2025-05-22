using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PrescriptionController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;
    private readonly ILogger<PrescriptionController> _logger;

    public PrescriptionController(
        IPrescriptionService prescriptionService,
        ILogger<PrescriptionController> logger)
    {
        _prescriptionService = prescriptionService;
        _logger = logger;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int prescriptionId = await _prescriptionService.AddPrescriptionAsync(request);

            return CreatedAtAction(nameof(AddPrescription), new { id = prescriptionId }, prescriptionId);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Błąd walidacji podczas dodawania recepty");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nieoczekiwany błąd podczas dodawania recepty");
            return StatusCode(StatusCodes.Status500InternalServerError, "Wystąpił błąd podczas przetwarzania żądania");
        }
    }
}