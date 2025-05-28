using System.ComponentModel.DataAnnotations;

namespace Apteka.DTOs;

public class NewPrescriptionDTO
{
    [Required] public int IdDoctor { get; set; }

    [Required] public NewPatientDTO Patient { get; set; } = null!;

    [Required] public DateTime Date { get; set; }
    [Required] public DateTime DueDate { get; set; }

    [Required] public ICollection<NewMedicamentsDTO> Medicaments { get; set; } = new HashSet<NewMedicamentsDTO>();
}

public class NewPatientDTO
{
    [Required] public int IdPatient { get; set; }
    [Required, MaxLength(100)] public string FirstName { get; set; } = null!;
    [Required, MaxLength(100)] public string LastName { get; set; } = null!;
    [Required] public DateTime BirthDate { get; set; }
}

public class NewMedicamentsDTO
{
    [Required] public int IdMedicament { get; set; }
    [Required, MaxLength(100)] public string Dose { get; set; } = null!;
    [Required, MaxLength(100)] public string Description { get; set; } = null!;
}