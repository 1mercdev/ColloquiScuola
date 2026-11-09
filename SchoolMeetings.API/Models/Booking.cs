using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolMeetings.API.Models.Enums;

namespace SchoolMeetings.API.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TimeSlotId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ParentUserId { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [MaxLength(1000)]
        public string? ParentNotes { get; set; }

        [MaxLength(1000)]
        public string? TeacherNotes { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public int? CancelledByUserId { get; set; }

        [ForeignKey(nameof(TimeSlotId))]
        public TimeSlot TimeSlot { get; set; } = null!;

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [ForeignKey(nameof(ParentUserId))]
        public User ParentUser { get; set; } = null!;

        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; } = null!;

        [ForeignKey(nameof(CancelledByUserId))]
        public User? CancelledByUser { get; set; }
    }
}
