using Microsoft.EntityFrameworkCore;
using Apteka.Models;

namespace Apteka.Data;

public class AptekaContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

    public AptekaContext(DbContextOptions<AptekaContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var patients = new List<Patient>
        {
            new() {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                BirthDate = DateTime.Parse("2003-12-06")
            },
            new() {
                IdPatient = 2,
                FirstName = "Anna",
                LastName = "Nowak",
                BirthDate = DateTime.Parse("2000-10-19")
            }
        };

        var doctors = new List<Doctor>
        {
            new() {
                IdDoctor = 1,
                FirstName = "Adam",
                LastName = "Nowak",
                Email = "adamn@gmail.com"
            },
            new() {
                IdDoctor = 2,
                FirstName = "Aleksandra",
                LastName = "Wiśniewska",
                Email = "alexawisnia@gmail.com"
            }
        };

        var medicaments = new List<Medicament>
        {
            new() {
                IdMedicament = 1,
                Name = "Apam",
                Description = "description1",
                Type = "A"
            },
            new() {
                IdMedicament = 2,
                Name = "Positiwum",
                Description = "Description2",
                Type = "B"
            }
        };

        var prescriptions = new List<Prescription>
        {
            new() {
                IdPrescription = 1,
                Date = DateTime.Parse("2024-05-28"),
                DueDate = DateTime.Parse("2024-05-29"),
                IdPatient = 1,
                IdDoctor = 2
            },
            new() {
                IdPrescription = 2,
                Date = DateTime.Parse("2024-02-28"),
                DueDate = DateTime.Parse("2024-03-29"),
                IdPatient = 2,
                IdDoctor = 1
            }
        };

        var prescriptionMedicaments = new List<PrescriptionMedicament>
        {
            new() {
                IdPrescription = 1,
                IdMedicament = 1,
                Dose = null,
                Details = "Details1"
            },
            new() {
                IdPrescription = 2,
                IdMedicament = 2,
                Dose = 20,
                Details = "Details2"
            }
        };

        modelBuilder.Entity<Patient>().HasData(patients);
        modelBuilder.Entity<Doctor>().HasData(doctors);
        modelBuilder.Entity<Medicament>().HasData(medicaments);
        modelBuilder.Entity<Prescription>().HasData(prescriptions);
        modelBuilder.Entity<PrescriptionMedicament>().HasData(prescriptionMedicaments);
    }
}

