using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.Bookings
{
    public class AddTeacherNotesDto
    {
        [Required]
        [MaxLength(1000)]
        public string TeacherNotes { get; set; } = string.Empty;
    }
}