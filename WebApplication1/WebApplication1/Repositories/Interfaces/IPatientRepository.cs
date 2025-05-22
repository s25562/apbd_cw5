using WebApplication1.Models;

namespace WebApplication1.Repositories.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient> GetWithPrescriptionsAsync(int id);
    Task<Patient> GetByEmailAsync(string email);
}