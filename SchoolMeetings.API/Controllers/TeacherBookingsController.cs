using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.DTOs.Bookings;
using SchoolMeetings.API.Services.Interfaces;
using System.Security.Claims;

namespace SchoolMeetings.API.Controllers
{
    [ApiController]
    [Route("api/teacher/bookings")]
    [Authorize(Roles = "Teacher")]
    public class TeacherBookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ApplicationDbContext _context;

        public TeacherBookingsController(IBookingService bookingService, ApplicationDbContext context)
        {
            _bookingService = bookingService;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TeacherBookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
                return NotFound(new { message = "Docente non trovato" });

            var bookings = await _bookingService.GetTeacherBookingsAsync(teacher.Id);
            return Ok(bookings);
        }

        [HttpPatch("{id}/notes")]
        [ProducesResponseType(typeof(TeacherBookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddNotes(int id, [FromBody] AddTeacherNotesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
                return NotFound(new { message = "Docente non trovato" });

            var booking = await _bookingService.AddTeacherNotesAsync(id, teacher.Id, dto.TeacherNotes);

            if (booking == null)
                return NotFound(new { message = "Prenotazione non trovata" });

            return Ok(booking);
        }
        [HttpPost("{id}/skip")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SkipBooking(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

            if (teacher == null)
                return NotFound(new { message = "Docente non trovato" });

            var result = await _bookingService.SkipBookingAsync(id, teacher.Id);

            if (!result)
                return BadRequest(new { message = "Impossibile saltare la prenotazione. Verifica che esista e sia confermata." });

            return NoContent();
        }
    }
}