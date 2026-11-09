using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.DTOs.Admin;
using SchoolMeetings.API.Models.Enums;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CleanupResultDto> CleanupSchoolDataAsync(int schoolId)
        {
            var bookingsToDelete = await _context.Bookings
                .Where(b => b.TimeSlot.Teacher.SchoolId == schoolId
                    && (b.Status == BookingStatus.Completed || b.Status == BookingStatus.Cancelled))
                .ToListAsync();

            _context.Bookings.RemoveRange(bookingsToDelete);
            var bookingsDeleted = bookingsToDelete.Count;

            var now = DateTime.Now;
            var timeSlotsToDelete = await _context.TimeSlots
                .Where(ts => ts.Teacher.SchoolId == schoolId
                    && ts.EndDateTime < now
                    && ts.Status != TimeSlotStatus.Booked)
                .ToListAsync();

            _context.TimeSlots.RemoveRange(timeSlotsToDelete);
            var timeSlotsDeleted = timeSlotsToDelete.Count;

            await _context.SaveChangesAsync();

            return new CleanupResultDto
            {
                BookingsDeleted = bookingsDeleted,
                TimeSlotsDeleted = timeSlotsDeleted,
                Message = $"Pulizia completata: {bookingsDeleted} prenotazioni e {timeSlotsDeleted} slot eliminati"
            };
        }
    }
}