using WebApplication1.DTOs;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Implementation;

public class PatientService: IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _logger = logger;
    }
public async Task<PatientDetailsResponse> GetPatientDetailsAsync(int idPatient)
        {
            var patient = await _patientRepository.GetWithPrescriptionsAsync(idPatient);
            if (patient == null)
            {
                throw new ArgumentException($"Pacjent o ID {idPatient} nie istnieje.");
            }

            var patientDto = new PatientDto
            {
                IdPatient = patient.IdPatient,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                BirthDate = patient.BirthDate
            };

            var prescriptionDtos = patient.Prescriptions
                .OrderBy(p => p.DueDate)
                .Select(p => new PrescriptionDetailsDto
                {
                    IdPrescription = p.IdPrescription,
                    Date = p.Date,
                    DueDate = p.DueDate,
                    Doctor = new DoctorDetailsDto
                    {
                        IdDoctor = p.Doctor.IdDoctor,
                        FirstName = p.Doctor.FirstName,
                        LastName = p.Doctor.LastName,
                        Email = p.Doctor.Email
                    },
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentDetailsDto
                    {
                        IdMedicament = pm.Medicament.IdMedicament,
                        Name = pm.Medicament.Name,
                        Dose = pm.Dose,
                        Description = pm.Medicament.Description,
                        Type = pm.Medicament.Type,
                        Details = pm.Details
                    }).ToList()
                })
                .ToList();

            var response = new PatientDetailsResponse
            {
                Patient = patientDto,
                Prescriptions = prescriptionDtos
            };

            _logger.LogInformation($"Pobrano dane pacjenta o ID {idPatient}");

            return response;
        }
}