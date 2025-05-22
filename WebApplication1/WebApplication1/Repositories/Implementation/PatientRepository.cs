using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<Patient> GetWithPrescriptionsAsync(int id)
    {
        return await _context.Patients
            .Include(p => p.Prescriptions)
            .ThenInclude(p => p.Doctor)
            .Include(p => p.Prescriptions)
            .ThenInclude(p => p.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Where(p => p.IdPatient == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Patient> GetByEmailAsync(string email)
    {
        return await _context.Patients
            .Where(p => p.Email == email)
            .FirstOrDefaultAsync();
    }
}