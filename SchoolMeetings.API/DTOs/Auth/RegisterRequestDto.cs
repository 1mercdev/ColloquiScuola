using System.ComponentModel.DataAnnotations;

namespace SchoolMeetings.API.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [MinLength(6, ErrorMessage = "La password deve essere di almeno 6 caratteri")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Il codice scuola è obbligatorio")]
        [MaxLength(50)]
        public string SchoolCode { get; set; } = string.Empty;
    }
}