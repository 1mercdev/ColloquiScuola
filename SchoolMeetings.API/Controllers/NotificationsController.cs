using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.DTOs.Notifications;
using SchoolMeetings.API.Models;
using System.Security.Claims;

namespace SchoolMeetings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterToken([FromBody] RegisterTokenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var existingToken = await _context.NotificationTokens
                .FirstOrDefaultAsync(t => t.Token == dto.Token);

            if (existingToken != null)
            {
                existingToken.UserId = userId;
                existingToken.Platform = dto.Platform;
                existingToken.LastUsedAt = DateTime.UtcNow;
            }
            else
            {
                var notificationToken = new NotificationToken
                {
                    UserId = userId,
                    Token = dto.Token,
                    Platform = dto.Platform,
                    CreatedAt = DateTime.UtcNow,
                    LastUsedAt = DateTime.UtcNow
                };

                _context.NotificationTokens.Add(notificationToken);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Token registrato con successo" });
        }

        [HttpDelete("unregister-token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnregisterToken([FromBody] UnregisterTokenDto dto)
        {
            var token = await _context.NotificationTokens
                .FirstOrDefaultAsync(t => t.Token == dto.Token);

            if (token != null)
            {
                _context.NotificationTokens.Remove(token);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}