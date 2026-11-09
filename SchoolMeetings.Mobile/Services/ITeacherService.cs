using SchoolMeetings.Mobile.Models;

namespace SchoolMeetings.Mobile.Services
{
    public interface ITeacherService
    {
        Task<bool> GenerateTimeSlotsAsync(GenerateTimeSlotsRequest request);
        Task<List<TeacherBooking>> GetMyBookingsAsync();
        Task<bool> AddNotesAsync(int bookingId, string notes);
        Task<bool> SkipBookingAsync(int bookingId);
    }
}