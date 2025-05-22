using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<Doctor> GetByEmailAsync(string email)
    {
        return await _context.Doctors
            .Where(d => d.Email == email)
            .FirstOrDefaultAsync();
    }
}