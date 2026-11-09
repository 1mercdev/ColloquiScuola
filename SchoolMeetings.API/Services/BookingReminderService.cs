using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.Models.Enums;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Services
{
    public class BookingReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingReminderService> _logger;

        public BookingReminderService(
            IServiceProvider serviceProvider,
            ILogger<BookingReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BookingReminderService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in BookingReminderService");
                }

                // Controlla ogni 5 minuti
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task SendRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var now = DateTime.Now;
            var reminderTime = now.AddMinutes(20);

            var bookingsToRemind = await context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.ParentUser)
                .Include(b => b.Teacher)
                    .ThenInclude(t => t.User)
                .Include(b => b.Student)
                .Where(b => b.Status == BookingStatus.Confirmed
                    && b.TimeSlot.StartDateTime >= reminderTime
                    && b.TimeSlot.StartDateTime <= reminderTime.AddMinutes(5))
                .ToListAsync();

            foreach (var booking in bookingsToRemind)
            {
                var title = "Promemoria Colloquio";
                var body = $"Il tuo colloquio con {booking.Teacher.User.FirstName} {booking.Teacher.User.LastName} " +
                           $"per {booking.Student.FirstName} inizierà tra 20 minuti!";

                var data = new Dictionary<string, string>
                {
                    { "type", "reminder" },
                    { "bookingId", booking.Id.ToString() },
                    { "teacherName", $"{booking.Teacher.User.FirstName} {booking.Teacher.User.LastName}" },
                    { "studentName", booking.Student.FirstName }
                };

                await notificationService.SendNotificationAsync(booking.ParentUserId, title, body, data);

                _logger.LogInformation($"Reminder sent for booking {booking.Id}");
            }
        }
    }
}