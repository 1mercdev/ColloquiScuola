using SchoolMeetings.API.DTOs.Bookings;

namespace SchoolMeetings.API.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetMyBookingsAsync(int parentUserId);
        Task<BookingDto?> GetBookingByIdAsync(int bookingId, int userId);
        Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, int parentUserId);
        Task<bool> CancelBookingAsync(int bookingId, int userId);
        Task<List<TeacherBookingDto>> GetTeacherBookingsAsync(int teacherId);
        Task<TeacherBookingDto?> AddTeacherNotesAsync(int bookingId, int teacherId, string notes);
        Task<bool> SkipBookingAsync(int bookingId, int teacherId);
    }
}