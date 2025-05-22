namespace WebApplication1.DTOs;

public class PrescriptionDetailsDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public DoctorDetailsDto Doctor { get; set; }
    public List<MedicamentDetailsDto> Medicaments { get; set; }
}
