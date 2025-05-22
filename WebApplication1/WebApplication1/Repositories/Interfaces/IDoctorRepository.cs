using WebApplication1.Models;

namespace WebApplication1.Repositories.Interfaces;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<Doctor> GetByEmailAsync(string email);
}