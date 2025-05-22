using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs;

public class AddPrescriptionRequest
{
    public PatientDto Patient { get; set; }
        
    public DoctorDto Doctor { get; set; }
        
    [Required]
    public DateTime Date { get; set; }
        
    [Required]
    public DateTime DueDate { get; set; }
        
    public List<MedicamentDto> Medicaments { get; set; }
}