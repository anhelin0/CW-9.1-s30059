using Microsoft.EntityFrameworkCore;
using Apteka.Data;
using Apteka.DTOs;
using Apteka.Exceptions;
using Apteka.Models;

namespace Apteka.Services;

public interface IDbService
{
    Task<PatientGetDTO> GetPatientAsync(int patientId);
    Task AddPrescriptionAsync(NewPrescriptionDTO dto);
}

public class DbService(AptekaContext db) : IDbService
{
    public async Task<PatientGetDTO> GetPatientAsync(int patientId)
    {
        var patient = await db.Patients
            .Include(p => p.Prescriptions.OrderBy(p => p.DueDate))
                .ThenInclude(p => p.PrescriptionMedicament)
                    .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
                .ThenInclude(p => p.Doctor)
            .FirstOrDefaultAsync(p => p.IdPatient == patientId);

        if (patient is null)
        {
            throw new NotFoundException($"Patient with id: {patientId} not found");
        }

        return new PatientGetDTO
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            BirthDate = patient.BirthDate,
            Prescriptions = patient.Prescriptions.Select(p => new PrescriptionsDTO
            {
                IdPrescription = p.IdPrescription,
                Date = p.Date,
                DueDate = p.DueDate,
                Doctor = new DoctorDTO
                {
                    IdDoctor = p.Doctor.IdDoctor,
                    FirstName = p.Doctor.FirstName
                },
                Medicaments = p.PrescriptionMedicament.Select(pm => new MedicamentsDTO
                {
                    IdMedicament = pm.Medicament.IdMedicament,
                    Name = pm.Medicament.Name,
                    Dose = pm.Dose,
                    Details = pm.Details
                }).ToList()
            }).ToList()
        };
    }

    public async Task AddPrescriptionAsync(NewPrescriptionDTO dto)
    {
        if (dto.Medicaments.Count > 10)
            throw new NotFoundException("A prescription can have a maximum of 10 medicaments.");

        if (dto.DueDate < dto.Date)
            throw new NotFoundException("DueDate cannot be earlier than Date.");

        var doctor = await db.Doctors.FindAsync(dto.IdDoctor)
            ?? throw new NotFoundException($"Doctor with ID {dto.IdDoctor} not found.");

        var patient = await db.Patients.FirstOrDefaultAsync(p =>
            p.IdPatient == dto.Patient.IdPatient &&
            p.FirstName == dto.Patient.FirstName &&
            p.LastName == dto.Patient.LastName &&
            p.BirthDate == dto.Patient.BirthDate);

        if (patient is null)
        {
            patient = new Patient
            {
                FirstName = dto.Patient.FirstName,
                LastName = dto.Patient.LastName,
                BirthDate = dto.Patient.BirthDate
            };
            await db.Patients.AddAsync(patient);
            await db.SaveChangesAsync();
        }

        var prescription = new Prescription
        {
            Date = dto.Date,
            DueDate = dto.DueDate,
            IdDoctor = dto.IdDoctor,
            IdPatient = patient.IdPatient,
            PrescriptionMedicament = new List<PrescriptionMedicament>()
        };

        foreach (var m in dto.Medicaments)
        {
            var medicament = await db.Medicaments.FindAsync(m.IdMedicament)
                ?? throw new NotFoundException($"Medicament with ID {m.IdMedicament} not found.");

            prescription.PrescriptionMedicament.Add(new PrescriptionMedicament
            {
                IdMedicament = m.IdMedicament,
                Dose = int.TryParse(m.Dose, out var parsedDose) ? parsedDose : null,
                Details = m.Description
            });
        }

        await db.Prescriptions.AddAsync(prescription);
        await db.SaveChangesAsync();
    }
}
