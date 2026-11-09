using Microsoft.AspNetCore.Mvc;
using SchoolMeetings.API.DTOs.Auth;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(request);

                if (result == null)
                    return Unauthorized(new { message = "Email o password non corretti" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Serverside login error: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RegisterParentAsync(request);

                if (result == null)
                    return BadRequest(new { message = "Codice scuola non valido o email già registrata" });

                return CreatedAtAction(nameof(Login), result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Serverside register error: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("register-admin")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RegisterAdminAsync(request);

                if (result == null)
                    return BadRequest(new { message = "Codice scuola non valido, email già registrata, o admin già esistente per questa scuola" });

                return CreatedAtAction(nameof(Login), result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Serverside register admin error: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}