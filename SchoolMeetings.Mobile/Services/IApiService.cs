using SchoolMeetings.Mobile.Models;

namespace SchoolMeetings.Mobile.Services
{
    public interface IApiService
    {
        Task<List<Teacher>> GetTeachersAsync();
        Task<bool> GetNotificationPreferenceAsync(int teacherId);
        Task<bool> SetNotificationPreferenceAsync(int teacherId, bool enabled);
    }
}