using WebApplication1.DTOs;

namespace WebApplication1.Services.Interfaces;

public interface IPatientService
{
    Task<PatientDetailsResponse> GetPatientDetailsAsync(int idPatient);
}