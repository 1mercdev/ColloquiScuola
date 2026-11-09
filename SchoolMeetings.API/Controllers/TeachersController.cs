using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMeetings.API.DTOs.Teacher;
using SchoolMeetings.API.DTOs.Teachers;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllTeachers([FromQuery] int schoolId = 1)
        {
            var teachers = await _teacherService.GetAllTeachersAsync(schoolId);
            return Ok(teachers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTeacherById(int id, [FromQuery] int schoolId = 1)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id, schoolId);

            if (teacher == null)
                return NotFound(new { message = "Docente non trovato" });

            return Ok(teacher);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var schoolId = int.Parse(User.FindFirst("SchoolId")!.Value);
                var teacher = await _teacherService.CreateTeacherAsync(dto, schoolId);
                return CreatedAtAction(nameof(GetTeacherById), new { id = teacher.Id }, teacher);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] UpdateTeacherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var schoolId = int.Parse(User.FindFirst("SchoolId")!.Value);
            var teacher = await _teacherService.UpdateTeacherAsync(id, dto, schoolId);

            if (teacher == null)
                return NotFound(new { message = "Docente non trovato" });

            return Ok(teacher);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var schoolId = int.Parse(User.FindFirst("SchoolId")!.Value);
            var result = await _teacherService.DeleteTeacherAsync(id, schoolId);

            if (!result)
                return NotFound(new { message = "Docente non trovato" });

            return NoContent();
        }
    }
}