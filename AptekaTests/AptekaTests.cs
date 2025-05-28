using Microsoft.EntityFrameworkCore;
using Apteka.Data;
using Apteka.DTOs;
using Apteka.Exceptions;
using Apteka.Models;
using Apteka.Services;
using Xunit;

namespace Apteka.Tests;

public class DbServiceTests
{
    private AptekaContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AptekaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AptekaContext(options);

        context.Doctors.Add(new Doctor
        {
            IdDoctor = 1,
            FirstName = "John",
            LastName = "Smith",
            Email = "john@example.com"
        });

        context.Patients.Add(new Patient
        {
            IdPatient = 1,
            FirstName = "Anna",
            LastName = "Nowak",
            BirthDate = DateTime.Parse("2000-01-01")
        });

        context.Medicaments.Add(new Medicament
        {
            IdMedicament = 1,
            Name = "Paracetamol",
            Description = "Painkiller",
            Type = "A"
        });

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task AddPrescriptionAsync_ShouldAddPrescriptionSuccessfully()
    {
        var db = GetDbContext();
        var service = new DbService(db);

        var dto = new NewPrescriptionDTO
        {
            IdDoctor = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(2),
            Patient = new NewPatientDTO
            {
                IdPatient = 1,
                FirstName = "Anna",
                LastName = "Nowak",
                BirthDate = DateTime.Parse("2000-01-01")
            },
            Medicaments = new List<NewMedicamentsDTO>
            {
                new() { IdMedicament = 1, Dose = "10", Description = "Take after meal" }
            }
        };

        await service.AddPrescriptionAsync(dto);
        Assert.Equal(1, db.Prescriptions.Count());
        Assert.Equal(1, db.PrescriptionMedicaments.Count());
    }

    [Fact]
    public async Task AddPrescriptionAsync_ShouldThrow_WhenTooManyMedicaments()
    {
        var db = GetDbContext();
        var service = new DbService(db);

        var dto = new NewPrescriptionDTO
        {
            IdDoctor = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(2),
            Patient = new NewPatientDTO
            {
                IdPatient = 1,
                FirstName = "Anna",
                LastName = "Nowak",
                BirthDate = DateTime.Parse("2000-01-01")
            },
            Medicaments = Enumerable.Range(0, 11).Select(_ => new NewMedicamentsDTO
            {
                IdMedicament = 1,
                Dose = "10",
                Description = "test"
            }).ToList()
        };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.AddPrescriptionAsync(dto));
        Assert.Equal("A prescription can have a maximum of 10 medicaments.", ex.Message);
    }

    [Fact]
    public async Task AddPrescriptionAsync_ShouldThrow_WhenDueDateEarlier()
    {
        var db = GetDbContext();
        var service = new DbService(db);

        var dto = new NewPrescriptionDTO
        {
            IdDoctor = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(-1),
            Patient = new NewPatientDTO
            {
                IdPatient = 1,
                FirstName = "Anna",
                LastName = "Nowak",
                BirthDate = DateTime.Parse("2000-01-01")
            },
            Medicaments = new List<NewMedicamentsDTO>
            {
                new() { IdMedicament = 1, Dose = "10", Description = "test" }
            }
        };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.AddPrescriptionAsync(dto));
        Assert.Equal("DueDate cannot be earlier than Date.", ex.Message);
    }

    [Fact]
    public async Task AddPrescriptionAsync_ShouldThrow_WhenDoctorNotFound()
    {
        var db = GetDbContext();
        var service = new DbService(db);

        var dto = new NewPrescriptionDTO
        {
            IdDoctor = 999,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(1),
            Patient = new NewPatientDTO
            {
                IdPatient = 1,
                FirstName = "Anna",
                LastName = "Nowak",
                BirthDate = DateTime.Parse("2000-01-01")
            },
            Medicaments = new List<NewMedicamentsDTO>
            {
                new() { IdMedicament = 1, Dose = "10", Description = "test" }
            }
        };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.AddPrescriptionAsync(dto));
        Assert.Contains("Doctor with ID", ex.Message);
    }

    [Fact]
    public async Task AddPrescriptionAsync_ShouldThrow_WhenMedicamentNotFound()
    {
        var db = GetDbContext();
        var service = new DbService(db);

        var dto = new NewPrescriptionDTO
        {
            IdDoctor = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(1),
            Patient = new NewPatientDTO
            {
                IdPatient = 1,
                FirstName = "Anna",
                LastName = "Nowak",
                BirthDate = DateTime.Parse("2000-01-01")
            },
            Medicaments = new List<NewMedicamentsDTO>
            {
                new() { IdMedicament = 999, Dose = "10", Description = "test" }
            }
        };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.AddPrescriptionAsync(dto));
        Assert.Contains("Medicament with ID", ex.Message);
    }

    [Fact]
    public async Task AddPrescriptionAsync_ShouldCreateNewPatient_IfNotExists()
    {
        var db = GetDbContext();
        var service = new DbService(db);

        var dto = new NewPrescriptionDTO
        {
            IdDoctor = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(1),
            Patient = new NewPatientDTO
            {
                IdPatient = 999,
                FirstName = "New",
                LastName = "Patient",
                BirthDate = DateTime.Parse("1990-01-01")
            },
            Medicaments = new List<NewMedicamentsDTO>
            {
                new() { IdMedicament = 1, Dose = "5", Description = "desc" }
            }
        };

        await service.AddPrescriptionAsync(dto);
        Assert.Contains(db.Patients, p => p.FirstName == "New" && p.LastName == "Patient");
    }

    [Fact]
    public async Task GetPatientAsync_ShouldReturnPatientData()
    {
        var db = GetDbContext();

        var prescription = new Prescription
        {
            IdPrescription = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(5),
            IdDoctor = 1,
            IdPatient = 1
        };
        db.Prescriptions.Add(prescription);
        db.PrescriptionMedicaments.Add(new PrescriptionMedicament
        {
            IdMedicament = 1,
            IdPrescription = 1,
            Details = "Take twice",
            Dose = 5
        });
        db.SaveChanges();

        var service = new DbService(db);

        var result = await service.GetPatientAsync(1);

        Assert.Equal("Anna", result.FirstName);
        Assert.Single(result.Prescriptions);
    }

    [Fact]
    public async Task GetPatientAsync_ShouldThrow_WhenPatientNotFound()
    {
        var db = GetDbContext();
        var service = new DbService(db);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => service.GetPatientAsync(999));
        Assert.Contains("not found", ex.Message.ToLower());
    }
}
