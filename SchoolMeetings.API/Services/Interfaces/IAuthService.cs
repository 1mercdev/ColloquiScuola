using SchoolMeetings.API.DTOs.Auth;

namespace SchoolMeetings.API.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Effettua il login di un utente
        /// </summary>
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

        /// <summary>
        /// Registra un nuovo genitore
        /// </summary>
        Task<LoginResponseDto?> RegisterParentAsync(RegisterRequestDto request);

        /// <summary>
        /// Genera un token JWT per un utente
        /// </summary>
        string GenerateJwtToken(int userId, string email, string role, int schoolId);

        /// <summary>
        /// Registra un nuovo admin di scuola
        /// </summary>
        Task<LoginResponseDto?> RegisterAdminAsync(RegisterRequestDto request);
    }
}