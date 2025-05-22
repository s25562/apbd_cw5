using WebApplication1.Models;

namespace WebApplication1.Repositories.Interfaces;

public interface IMedicamentRepository : IGenericRepository<Medicament>
{
    Task<Medicament> GetByNameAsync(string name);
    Task<bool> ExistsAsync(int id);
}