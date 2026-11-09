using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolMeetings.API.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ParentUserId { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Class { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ParentUserId))]
        public User ParentUser { get; set; } = null!;

        [ForeignKey(nameof(SchoolId))]
        public School School { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}