using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.Notifications
{
    public class RegisterTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Platform { get; set; } = string.Empty; // "Android" or "iOS"
    }

    public class UnregisterTokenDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}