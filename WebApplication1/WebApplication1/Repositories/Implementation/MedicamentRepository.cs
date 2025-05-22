using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public class MedicamentRepository: GenericRepository<Medicament>, IMedicamentRepository
{
    public MedicamentRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<Medicament> GetByNameAsync(string name)
    {
        return await _context.Medicaments
            .Where(m => m.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Medicaments.AnyAsync(m => m.IdMedicament == id);
    }

}