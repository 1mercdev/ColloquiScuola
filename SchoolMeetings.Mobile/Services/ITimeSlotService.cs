using SchoolMeetings.Mobile.Models;

namespace SchoolMeetings.Mobile.Services
{
    public interface ITimeSlotService
    {
        Task<List<TimeSlot>> GetTodaySlotsByTeacherAsync(int teacherId);
        Task<TimeSlot?> GetSlotByIdAsync(int slotId);
    }
}