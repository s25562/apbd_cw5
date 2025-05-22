using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs;

public class PatientDto
{
    public int? IdPatient { get; set; } // Opcjonalne, jeśli tworzymy nowego pacjenta
        
    [Required]
    public string FirstName { get; set; }
        
    [Required]
    public string LastName { get; set; }
        
    [Required]
    [EmailAddress]
    public string Email { get; set; }
        
    public DateTime BirthDate { get; set; }
}