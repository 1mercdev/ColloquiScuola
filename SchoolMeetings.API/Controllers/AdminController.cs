using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMeetings.API.DTOs.Admin;
using SchoolMeetings.API.Services.Interfaces;
using System.Security.Claims;

namespace SchoolMeetings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("cleanup")]
        [ProducesResponseType(typeof(CleanupResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CleanupSchoolData()
        {
            var schoolId = int.Parse(User.FindFirst("SchoolId")!.Value);
            var result = await _adminService.CleanupSchoolDataAsync(schoolId);
            return Ok(result);
        }
    }
}