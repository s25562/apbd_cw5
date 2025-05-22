using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories;

public interface IPrescriptionRepository : IGenericRepository<Prescription>
{
    Task<Prescription> GetWithDetailsAsync(int id);
}