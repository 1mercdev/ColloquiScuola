using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.DTOs.TimeSlots;
using SchoolMeetings.API.Services.Interfaces;
using System.Security.Claims;

namespace SchoolMeetings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TimeSlotsController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;
        private readonly ApplicationDbContext _context;

        public TimeSlotsController(ITimeSlotService timeSlotService, ApplicationDbContext context)
        {
            _timeSlotService = timeSlotService;
            _context = context;
        }

        [HttpGet("teacher/{teacherId}")]
        [ProducesResponseType(typeof(List<TimeSlotDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAvailableSlotsByTeacher(int teacherId, [FromQuery] DateTime? fromDate = null)
        {
            var slots = await _timeSlotService.GetAvailableSlotsByTeacherAsync(teacherId, fromDate);
            return Ok(slots);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TimeSlotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSlotById(int id)
        {
            var slot = await _timeSlotService.GetSlotByIdAsync(id);

            if (slot == null)
                return NotFound(new { message = "Slot non trovato" });

            return Ok(slot);
        }
        [HttpPost("generate")]
        [Authorize(Roles = "Teacher,Admin")]
        [ProducesResponseType(typeof(GenerateTimeSlotsResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GenerateSlots([FromBody] GenerateTimeSlotsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                int teacherId;

                if (role == "Teacher")
                {
                    var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
                    if (teacher == null)
                        return NotFound(new { message = "Docente non trovato" });
                    teacherId = teacher.Id;
                }
                else if (role == "Admin")
                {
                    return BadRequest(new { message = "Admin deve specificare teacherId" });
                }
                else
                {
                    return Forbid();
                }

                var result = await _timeSlotService.GenerateSlotsAsync(teacherId, dto);
                return CreatedAtAction(nameof(GetAvailableSlotsByTeacher), new { teacherId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}