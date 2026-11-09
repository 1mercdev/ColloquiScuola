using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required]
        public int TimeSlotId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [MaxLength(1000)]
        public string? ParentNotes { get; set; }
    }
}