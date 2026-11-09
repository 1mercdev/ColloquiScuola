using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.Teachers
{
    public class UpdateTeacherDto
    {
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

        [Range(5, 60)]
        public int SlotDurationMinutes { get; set; } = 15;

        [Range(0, 50)]
        public int MaxDailyBookings { get; set; } = 0;

        public bool IsAvailable { get; set; } = true;
    }
}