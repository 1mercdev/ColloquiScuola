using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.Models.Enums;

namespace SchoolMeetings.API.BackgroundJobs
{
    public class CompleteBookingsJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompleteBookingsJob> _logger;

        public CompleteBookingsJob(ApplicationDbContext context, ILogger<CompleteBookingsJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Run()
        {
            _logger.LogInformation("CompleteBookingsJob started at {Time}", DateTime.UtcNow);

            var pastBookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Where(b => b.Status == BookingStatus.Confirmed
                    && b.TimeSlot.EndDateTime < DateTime.Now)
                .ToListAsync();

            if (pastBookings.Any())
            {
                foreach (var booking in pastBookings)
                {
                    booking.Status = BookingStatus.Completed;
                    booking.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Completed {Count} past bookings", pastBookings.Count);
            }
            else
            {
                _logger.LogInformation("No past bookings to complete");
            }
        }
    }
}