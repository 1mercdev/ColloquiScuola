using SchoolMeetings.Mobile.Models;

namespace SchoolMeetings.Mobile.Services
{
    public interface IAdminService
    {
        Task<List<Teacher>> GetAllTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int teacherId);
        Task<Teacher?> CreateTeacherAsync(CreateTeacherRequest request);
        Task<Teacher?> UpdateTeacherAsync(int teacherId, UpdateTeacherRequest request);
        Task<bool> DeleteTeacherAsync(int teacherId);
        Task<CleanupResult?> CleanupDatabaseAsync();
    }
}