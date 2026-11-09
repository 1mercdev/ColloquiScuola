using SchoolMeetings.API.DTOs.Admin;

namespace SchoolMeetings.API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<CleanupResultDto> CleanupSchoolDataAsync(int schoolId);
    }
}