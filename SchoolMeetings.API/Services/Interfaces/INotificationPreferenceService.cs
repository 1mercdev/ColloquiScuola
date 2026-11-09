namespace SchoolMeetings.API.Services.Interfaces
{
    public interface INotificationPreferenceService
    {
        Task NotifyUsersAboutNewSlotsAsync(int teacherId, DateTime slotDate);
        Task NotifySlotBecameAvailableAsync(int teacherId, DateTime slotTime);
    }
}
