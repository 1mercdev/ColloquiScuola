using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [MinLength(6, ErrorMessage = "La password deve essere di almeno 6 caratteri")]
        public string Password { get; set; } = string.Empty;
    }
}