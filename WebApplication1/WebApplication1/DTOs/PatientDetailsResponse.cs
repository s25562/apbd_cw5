namespace WebApplication1.DTOs;

public class PatientDetailsResponse
{
    public PatientDto Patient { get; set; }
    public List<PrescriptionDetailsDto> Prescriptions { get; set; }
}