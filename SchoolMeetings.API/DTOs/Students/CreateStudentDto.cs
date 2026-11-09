using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.Students
{
    public class CreateStudentDto
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La classe è obbligatoria")]
        [MaxLength(10)]
        public string Class { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}