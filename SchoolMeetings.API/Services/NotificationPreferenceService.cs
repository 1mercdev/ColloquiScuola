using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Services
{
    public class NotificationPreferenceService : INotificationPreferenceService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public NotificationPreferenceService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task NotifyUsersAboutNewSlotsAsync(int teacherId, DateTime slotDate)
        {
            var preferences = await _context.NotificationPreferences
                .Include(np => np.User)
                .Include(np => np.Teacher)
                .Where(np => np.TeacherId == teacherId && np.NotifyOnSlotAvailable)
                .ToListAsync();

            if (preferences.Count == 0) return;

            var userIds = preferences.Select(np => np.UserId).ToList();

            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher == null) return;

            await _notificationService.SendNotificationToMultipleAsync(
                    userIds,
                    "Nuovi slot disponibili!",
                    $"{teacher.User.FullName} - Nuovi slot disponibili per il {slotDate:dd/MM/yyyy}"
            );
        }

        public async Task NotifySlotBecameAvailableAsync(int teacherId, DateTime slotTime)
        {
            var preferences = await _context.NotificationPreferences
                .Where(np => np.TeacherId == teacherId && np.NotifyOnSlotAvailable)
                .ToListAsync();

            if (preferences.Count == 0) return;

            var userIds = preferences.Select(np => np.UserId).ToList();

            var teacher = await _context.Teachers.Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            await _notificationService.SendNotificationToMultipleAsync(
                    userIds,
                    "Slot disponibile!",
                    $"{teacher?.User.FullName} - Slot libero alle {slotTime:HH:mm}"
            );
        }
    }
}