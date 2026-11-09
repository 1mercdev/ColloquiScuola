using SchoolMeetings.Mobile.Models;

namespace SchoolMeetings.Mobile.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetMyBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<Booking?> CreateBookingAsync(CreateBookingRequest request);
        Task<bool> CancelBookingAsync(int bookingId);
    }
}