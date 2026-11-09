using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.Models;
using SchoolMeetings.API.DTOs.Notifications;
using System.Net;

namespace SchoolMeetings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationPreferenceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public NotificationPreferenceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("teacher/{teacherId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> GetPreference(int teacherId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var preference = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId && np.TeacherId == teacherId);

            return Ok(preference?.NotifyOnSlotAvailable ?? false);
        }

        [HttpPost("setnotificationpreference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SetPreference([FromBody] SetNotificationPreferenceDto dto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var preference = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId && np.TeacherId == dto.TeacherId);

            if (preference == null)
            {
                preference = new NotificationPreference
                {
                    UserId = userId,
                    TeacherId = dto.TeacherId,
                    NotifyOnSlotAvailable = dto.Enabled,
                    CreatedAt = DateTime.UtcNow
                };
                _context.NotificationPreferences.Add(preference);
            }
            else
            {
                preference.NotifyOnSlotAvailable = dto.Enabled;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
