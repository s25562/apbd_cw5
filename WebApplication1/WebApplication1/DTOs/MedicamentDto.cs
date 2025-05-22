using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs;

public class MedicamentDto
{
    [Required]
    public int IdMedicament { get; set; }
        
    public string Name { get; set; }
        
    [Required]
    public int Dose { get; set; }
        
    public string Description { get; set; }
}