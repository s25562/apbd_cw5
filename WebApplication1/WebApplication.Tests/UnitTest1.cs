using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Repositories;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Implementation;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;
using FluentAssertions;


namespace WebApplication.Tests;

public class PrescriptionServiceTests
{
    private readonly Mock<IPrescriptionRepository> _prescriptionRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly Mock<IMedicamentRepository> _medicamentRepositoryMock;
    private readonly Mock<ILogger<PrescriptionService>> _loggerMock;
    private readonly PrescriptionService _service;
    
    public PrescriptionServiceTests()
    {
        _prescriptionRepositoryMock = new Mock<IPrescriptionRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _medicamentRepositoryMock = new Mock<IMedicamentRepository>();
        _loggerMock = new Mock<ILogger<PrescriptionService>>();

        _service = new PrescriptionService(
            _prescriptionRepositoryMock.Object,
            _patientRepositoryMock.Object,
            _doctorRepositoryMock.Object,
            _medicamentRepositoryMock.Object,
            _loggerMock.Object
        );
    }
    
    [Fact]
        public async Task AddPrescriptionAsync_WithValidData_ShouldAddPrescription()
        {
            var request = new AddPrescriptionRequest
            {
                Patient = new PatientDto
                {
                    IdPatient = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    BirthDate = new DateTime(1980, 1, 1)
                },
                Doctor = new DoctorDto
                {
                    IdDoctor = 1,
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Email = "anna.nowak@example.com"
                },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                Medicaments = new List<MedicamentDto>
                {
                    new MedicamentDto { IdMedicament = 1, Dose = 10, Description = "Raz dziennie" }
                }
            };

            var patient = new Patient
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                BirthDate = new DateTime(1980, 1, 1)
            };

            var doctor = new Doctor
            {
                IdDoctor = 1,
                FirstName = "Anna",
                LastName = "Nowak",
                Email = "anna.nowak@example.com"
            };

            var prescription = new Prescription
            {
                IdPrescription = 1,
                Date = request.Date,
                DueDate = request.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor = doctor.IdDoctor
            };

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(patient);
            _doctorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(doctor);
            _medicamentRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _prescriptionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Prescription>()))
                .Callback<Prescription>(p => p.IdPrescription = 1);

            // Act
            var result = await _service.AddPrescriptionAsync(request);

            // Assert
            result.Should().Be(1);
            _prescriptionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Prescription>()), Times.Once);
            _prescriptionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task AddPrescriptionAsync_WithInvalidDueDate_ShouldThrowArgumentException()
        {
            // Arrange
            var request = new AddPrescriptionRequest
            {
                Patient = new PatientDto
                {
                    IdPatient = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    BirthDate = new DateTime(1980, 1, 1)
                },
                Doctor = new DoctorDto
                {
                    IdDoctor = 1,
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Email = "anna.nowak@example.com"
                },
                Date = DateTime.Now.AddDays(10), // Data wystawienia w przyszłości
                DueDate = DateTime.Now, // Data ważności wcześniejsza niż data wystawienia
                Medicaments = new List<MedicamentDto>
                {
                    new MedicamentDto { IdMedicament = 1, Dose = 10, Description = "Raz dziennie" }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddPrescriptionAsync(request));
        }

        [Fact]
        public async Task AddPrescriptionAsync_WithTooManyMedicaments_ShouldThrowArgumentException()
        {
            // Arrange
            var request = new AddPrescriptionRequest
            {
                Patient = new PatientDto
                {
                    IdPatient = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    BirthDate = new DateTime(1980, 1, 1)
                },
                Doctor = new DoctorDto
                {
                    IdDoctor = 1,
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Email = "anna.nowak@example.com"
                },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                Medicaments = new List<MedicamentDto>()
            };

            // Dodajemy 11 leków (przekraczamy limit 10)
            for (int i = 1; i <= 11; i++)
            {
                request.Medicaments.Add(new MedicamentDto { IdMedicament = i, Dose = 10, Description = $"Lek {i}" });
            }

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddPrescriptionAsync(request));
        }

        [Fact]
        public async Task AddPrescriptionAsync_WithNonExistentMedicament_ShouldThrowArgumentException()
        {
            var request = new AddPrescriptionRequest
            {
                Patient = new PatientDto
                {
                    IdPatient = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    BirthDate = new DateTime(1980, 1, 1)
                },
                Doctor = new DoctorDto
                {
                    IdDoctor = 1,
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Email = "anna.nowak@example.com"
                },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                Medicaments = new List<MedicamentDto>
                {
                    new MedicamentDto { IdMedicament = 999, Dose = 10, Description = "Raz dziennie" } // Nieistniejący lek
                }
            };

            var patient = new Patient
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                BirthDate = new DateTime(1980, 1, 1)
            };

            var doctor = new Doctor
            {
                IdDoctor = 1,
                FirstName = "Anna",
                LastName = "Nowak",
                Email = "anna.nowak@example.com"
            };

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(patient);
            _doctorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(doctor);
            _medicamentRepositoryMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false); // Lek nie istnieje

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddPrescriptionAsync(request));
        }
    }

    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<ILogger<PatientService>> _loggerMock;
        private readonly PatientService _service;

        public PatientServiceTests()
        {
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _loggerMock = new Mock<ILogger<PatientService>>();

            _service = new PatientService(
                _patientRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetPatientDetailsAsync_WithValidId_ShouldReturnPatientDetails()
        {
            var patientId = 1;
            var patient = new Patient
            {
                IdPatient = patientId,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                BirthDate = new DateTime(1980, 1, 1),
                Prescriptions = new List<Prescription>
                {
                    new Prescription
                    {
                        IdPrescription = 1,
                        Date = DateTime.Now.AddDays(-10),
                        DueDate = DateTime.Now.AddDays(20),
                        Doctor = new Doctor
                        {
                            IdDoctor = 1,
                            FirstName = "Anna",
                            LastName = "Nowak",
                            Email = "anna.nowak@example.com"
                        },
                        PrescriptionMedicaments = new List<PrescriptionMedicament>
                        {
                            new PrescriptionMedicament
                            {
                                IdPrescription = 1,
                                IdMedicament = 1,
                                Dose = 10,
                                Details = "Raz dziennie",
                                Medicament = new Medicament
                                {
                                    IdMedicament = 1,
                                    Name = "Aspiryna",
                                    Description = "Lek przeciwbólowy",
                                    Type = "Tabletki"
                                }
                            }
                        }
                    }
                }
            };

            _patientRepositoryMock.Setup(r => r.GetWithPrescriptionsAsync(patientId)).ReturnsAsync(patient);

            var result = await _service.GetPatientDetailsAsync(patientId);

            result.Should().NotBeNull();
            result.Patient.IdPatient.Should().Be(patientId);
            result.Patient.FirstName.Should().Be("Jan");
            result.Patient.LastName.Should().Be("Kowalski");
            result.Prescriptions.Should().HaveCount(1);
            result.Prescriptions[0].Medicaments.Should().HaveCount(1);
            result.Prescriptions[0].Medicaments[0].Name.Should().Be("Aspiryna");
        }

        [Fact]
        public async Task GetPatientDetailsAsync_WithInvalidId_ShouldThrowArgumentException()
        {
            var patientId = 999; 
            _patientRepositoryMock.Setup(r => r.GetWithPrescriptionsAsync(patientId)).ReturnsAsync((Patient)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetPatientDetailsAsync(patientId));
        }
    }

    public class PrescriptionControllerTests
    {
        private readonly Mock<IPrescriptionService> _prescriptionServiceMock;
        private readonly Mock<ILogger<PrescriptionController>> _loggerMock;
        private readonly PrescriptionController _controller;

        public PrescriptionControllerTests()
        {
            _prescriptionServiceMock = new Mock<IPrescriptionService>();
            _loggerMock = new Mock<ILogger<PrescriptionController>>();

            _controller = new PrescriptionController(
                _prescriptionServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddPrescription_WithValidRequest_ShouldReturnCreatedAtAction()
        {
            var request = new AddPrescriptionRequest
            {
                Patient = new PatientDto
                {
                    IdPatient = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    BirthDate = new DateTime(1980, 1, 1)
                },
                Doctor = new DoctorDto
                {
                    IdDoctor = 1,
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Email = "anna.nowak@example.com"
                },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                Medicaments = new List<MedicamentDto>
                {
                    new MedicamentDto { IdMedicament = 1, Dose = 10, Description = "Raz dziennie" }
                }
            };

            _prescriptionServiceMock.Setup(s => s.AddPrescriptionAsync(request)).ReturnsAsync(1);

            var result = await _controller.AddPrescription(request);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            createdAtActionResult.ActionName.Should().Be(nameof(PrescriptionController.AddPrescription));
            createdAtActionResult.RouteValues["id"].Should().Be(1);
            createdAtActionResult.Value.Should().Be(1);
        }

        [Fact]
        public async Task AddPrescription_WithArgumentException_ShouldReturnBadRequest()
        {
            var request = new AddPrescriptionRequest
            {
                Patient = new PatientDto
                {
                    IdPatient = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    BirthDate = new DateTime(1980, 1, 1)
                },
                Doctor = new DoctorDto
                {
                    IdDoctor = 1,
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Email = "anna.nowak@example.com"
                },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                Medicaments = new List<MedicamentDto>
                {
                    new MedicamentDto { IdMedicament = 999, Dose = 10, Description = "Raz dziennie" } // Nieistniejący lek
                }
            };

            _prescriptionServiceMock.Setup(s => s.AddPrescriptionAsync(request))
                .ThrowsAsync(new ArgumentException("Lek o ID 999 nie istnieje."));

            var result = await _controller.AddPrescription(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().Be("Lek o ID 999 nie istnieje.");
        }
    }

    public class PatientControllerTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<PatientController>> _loggerMock;
        private readonly PatientController _controller;

        public PatientControllerTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<PatientController>>();

            _controller = new PatientController(
                _patientServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetPatientDetails_WithValidId_ShouldReturnOk()
        {
            var patientId = 1;
            var patientResponse = new PatientDetailsResponse
            {
                Patient = new PatientDto
                {
                    IdPatient = patientId,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    BirthDate = new DateTime(1980, 1, 1)
                },
                Prescriptions = new List<PrescriptionDetailsDto>
                {
                    new PrescriptionDetailsDto
                    {
                        IdPrescription = 1,
                        Date = DateTime.Now.AddDays(-10),
                        DueDate = DateTime.Now.AddDays(20),
                        Doctor = new DoctorDetailsDto
                        {
                            IdDoctor = 1,
                            FirstName = "Anna",
                            LastName = "Nowak",
                            Email = "anna.nowak@example.com"
                        },
                        Medicaments = new List<MedicamentDetailsDto>
                        {
                            new MedicamentDetailsDto
                            {
                                IdMedicament = 1,
                                Name = "Aspiryna",
                                Dose = 10,
                                Description = "Lek przeciwbólowy",
                                Type = "Tabletki",
                                Details = "Raz dziennie"
                            }
                        }
                    }
                }
            };

            _patientServiceMock.Setup(s => s.GetPatientDetailsAsync(patientId)).ReturnsAsync(patientResponse);

            var result = await _controller.GetPatientDetails(patientId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PatientDetailsResponse>(okResult.Value);
            returnValue.Patient.IdPatient.Should().Be(patientId);
            returnValue.Prescriptions.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetPatientDetails_WithInvalidId_ShouldReturnNotFound()
        {
            var patientId = 999;
            _patientServiceMock.Setup(s => s.GetPatientDetailsAsync(patientId))
                .ThrowsAsync(new ArgumentException($"Pacjent o ID {patientId} nie istnieje."));

            var result = await _controller.GetPatientDetails(patientId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            notFoundResult.Value.Should().Be($"Pacjent o ID {patientId} nie istnieje.");
        }

        [Fact]
        public async Task GetPatientDetails_WithZeroId_ShouldReturnBadRequest()
        {
            var patientId = 0;

            var result = await _controller.GetPatientDetails(patientId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().Be("ID pacjenta musi być liczbą większą od zera");
        }
    
}