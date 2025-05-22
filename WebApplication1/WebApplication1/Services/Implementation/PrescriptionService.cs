using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Implementation;

public class PrescriptionService: IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMedicamentRepository _medicamentRepository;
    private readonly ILogger<PrescriptionService> _logger;

    private const int MAX_MEDICAMENTS = 10;

    public PrescriptionService(
        IPrescriptionRepository prescriptionRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        IMedicamentRepository medicamentRepository,
        ILogger<PrescriptionService> logger)
    {
        _prescriptionRepository = prescriptionRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _medicamentRepository = medicamentRepository;
        _logger = logger;
    }
    
    public async Task<int> AddPrescriptionAsync(AddPrescriptionRequest request)
        {
            if (request.DueDate < request.Date)
            {
                throw new ArgumentException("Data ważności nie może być wcześniejsza niż data wystawienia recepty.");
            }

            if (request.Medicaments == null || request.Medicaments.Count == 0)
            {
                throw new ArgumentException("Recepta musi zawierać co najmniej jeden lek.");
            }

            if (request.Medicaments.Count > MAX_MEDICAMENTS)
            {
                throw new ArgumentException($"Recepta może zawierać maksymalnie {MAX_MEDICAMENTS} leków.");
            }

            var doctor = await _doctorRepository.GetByIdAsync(request.Doctor.IdDoctor);
            if (doctor == null)
            {
                throw new ArgumentException($"Lekarz o ID {request.Doctor.IdDoctor} nie istnieje.");
            }

            Patient patient;
            if (request.Patient.IdPatient.HasValue)
            {
                patient = await _patientRepository.GetByIdAsync(request.Patient.IdPatient.Value);
                if (patient == null)
                {
                    throw new ArgumentException($"Pacjent o ID {request.Patient.IdPatient.Value} nie istnieje.");
                }
            }
            else
            {
                patient = await _patientRepository.GetByEmailAsync(request.Patient.Email);
                if (patient == null)
                {
                    patient = new Patient
                    {
                        FirstName = request.Patient.FirstName,
                        LastName = request.Patient.LastName,
                        Email = request.Patient.Email,
                        BirthDate = request.Patient.BirthDate
                    };

                    await _patientRepository.AddAsync(patient);
                    await _patientRepository.SaveChangesAsync();
                }
            }

            foreach (var medicamentDto in request.Medicaments)
            {
                bool medicamentExists = await _medicamentRepository.ExistsAsync(medicamentDto.IdMedicament);
                if (!medicamentExists)
                {
                    throw new ArgumentException($"Lek o ID {medicamentDto.IdMedicament} nie istnieje.");
                }
            }

            var prescription = new Prescription
            {
                Date = request.Date,
                DueDate = request.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor = doctor.IdDoctor,
                PrescriptionMedicaments = new List<PrescriptionMedicament>()
            };

            await _prescriptionRepository.AddAsync(prescription);
            await _prescriptionRepository.SaveChangesAsync();

            foreach (var medicamentDto in request.Medicaments)
            {
                var prescriptionMedicament = new PrescriptionMedicament
                {
                    IdPrescription = prescription.IdPrescription,
                    IdMedicament = medicamentDto.IdMedicament,
                    Dose = medicamentDto.Dose,
                    Details = medicamentDto.Description
                };

                prescription.PrescriptionMedicaments.Add(prescriptionMedicament);
            }

            await _prescriptionRepository.SaveChangesAsync();

            _logger.LogInformation($"Dodano nową receptę o ID {prescription.IdPrescription}");

            return prescription.IdPrescription;
        }
}