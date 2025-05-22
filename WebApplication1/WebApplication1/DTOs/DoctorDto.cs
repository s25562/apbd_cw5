using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs;

public class DoctorDto
{
    [Required]
    public int IdDoctor { get; set; }
        
    public string FirstName { get; set; }
        
    public string LastName { get; set; }
        
    public string Email { get; set; }

}