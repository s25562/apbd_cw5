using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

public class Prescription
{
    [Key]
    public int IdPrescription { get; set; }
        
    // Data wystawienia recepty
    [Required]
    public DateTime Date { get; set; }
        
    // Data ważności recepty
    [Required]
    public DateTime DueDate { get; set; }
        
    public int IdPatient { get; set; }
        
    public int IdDoctor { get; set; }
        
    [ForeignKey("IdPatient")]
    public virtual Patient Patient { get; set; }
        
    [ForeignKey("IdDoctor")]
    public virtual Doctor Doctor { get; set; }
        
    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; } = new List<PrescriptionMedicament>();
}