using WebApplication1.DTOs;

namespace WebApplication1.Services.Interfaces;

public interface IPrescriptionService
{
    Task<int> AddPrescriptionAsync(AddPrescriptionRequest request);
}