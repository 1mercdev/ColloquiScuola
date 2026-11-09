using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolMeetings.API.Models.Enums;

namespace SchoolMeetings.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SchoolId { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        [ForeignKey(nameof(SchoolId))]
        public School School { get; set; } = null!;

        public Teacher? Teacher { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<NotificationToken> NotificationTokens { get; set; } = new List<NotificationToken>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}