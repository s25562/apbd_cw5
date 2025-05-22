using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class PrescriptionRepository: GenericRepository<Prescription>, IPrescriptionRepository
{
    public PrescriptionRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<Prescription> GetWithDetailsAsync(int id)
    {
        return await _context.Prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Where(p => p.IdPrescription == id)
            .FirstOrDefaultAsync();
    }
}