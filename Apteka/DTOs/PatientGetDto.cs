using System.ComponentModel.DataAnnotations;

namespace Apteka.DTOs;

public class PatientGetDTO
{
    [Required] public int IdPatient { get; set; }
    [Required, MaxLength(100)] public string FirstName { get; set; } = null!;
    [Required, MaxLength(100)] public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public IEnumerable<PrescriptionsDTO> Prescriptions { get; set; } = new HashSet<PrescriptionsDTO>();
}

public class PrescriptionsDTO
{
    [Required] public int IdPrescription { get; set; }
    [Required] public DateTime Date { get; set; }
    [Required] public DateTime DueDate { get; set; }
    [Required] public DoctorDTO Doctor { get; set; } = null!;
    public IEnumerable<MedicamentsDTO> Medicaments { get; set; } = new HashSet<MedicamentsDTO>();
}

public class MedicamentsDTO
{
    [Required] public int IdMedicament { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = null!;
    public int? Dose { get; set; }
    [Required, MaxLength(100)] public string Details { get; set; } = null!;
}

public class DoctorDTO
{
    [Required] public int IdDoctor { get; set; }
    [Required, MaxLength(100)] public string FirstName { get; set; } = null!;
}