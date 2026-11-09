using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.Teachers
{
    public class CreateTeacherDto
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "La materia è obbligatoria")]
        [MaxLength(100)]
        public string Subject { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Classroom { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Range(0, 50)]
        public int MaxDailyBookings { get; set; } = 0;
    }
}