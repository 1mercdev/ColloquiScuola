using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolMeetings.API.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Subject { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Classroom { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int SlotDurationMinutes { get; set; } = 15;

        public int MaxDailyBookings { get; set; } = 0;

        public bool IsAvailable { get; set; } = true;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(SchoolId))]
        public School School { get; set; } = null!;

        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}