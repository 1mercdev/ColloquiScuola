using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.DTOs.Bookings;
using SchoolMeetings.API.Models;
using SchoolMeetings.API.Models.Enums;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationPreferenceService _notificationPreferenceService;

        public BookingService(ApplicationDbContext context, IServiceProvider serviceProvider, INotificationPreferenceService notificationPreferenceService)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _notificationPreferenceService = notificationPreferenceService;
        }

        public async Task<List<BookingDto>> GetMyBookingsAsync(int parentUserId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.User)
                .Include(b => b.Student)
                .Where(b => b.ParentUserId == parentUserId && b.Status == BookingStatus.Confirmed)
                .OrderBy(b => b.TimeSlot.StartDateTime)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    TimeSlotId = b.TimeSlotId,
                    MeetingDateTime = b.TimeSlot.StartDateTime,
                    DurationMinutes = b.TimeSlot.DurationMinutes,
                    TeacherId = b.TeacherId,
                    TeacherName = b.Teacher.User.FirstName + " " + b.Teacher.User.LastName,
                    Subject = b.Teacher.Subject,
                    StudentId = b.StudentId,
                    StudentName = b.Student.FirstName + " " + b.Student.LastName,
                    StudentClass = b.Student.Class,
                    ParentNotes = b.ParentNotes,
                    TeacherNotes = b.TeacherNotes,
                    Status = b.Status.ToString(),
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return bookings;
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int bookingId, int userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .ThenInclude(ts => ts.Teacher)
                .ThenInclude(t => t.User)
                .Include(b => b.Student)
                .Where(b => b.Id == bookingId && b.ParentUserId == userId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    TimeSlotId = b.TimeSlotId,
                    MeetingDateTime = b.TimeSlot.StartDateTime,
                    DurationMinutes = b.TimeSlot.DurationMinutes,
                    TeacherId = b.TeacherId,
                    TeacherName = b.Teacher.User.FirstName + " " + b.Teacher.User.LastName,
                    Subject = b.Teacher.Subject,
                    StudentId = b.StudentId,
                    StudentName = b.Student.FirstName + " " + b.Student.LastName,
                    StudentClass = b.Student.Class,
                    ParentNotes = b.ParentNotes,
                    TeacherNotes = b.TeacherNotes,
                    Status = b.Status.ToString(),
                    CreatedAt = b.CreatedAt
                })
                .FirstOrDefaultAsync();

            return booking;
        }

        public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, int parentUserId)
        { 
            var timeSlot = await _context.TimeSlots
                .Include(ts => ts.Teacher)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(ts => ts.Id == dto.TimeSlotId);

            if (timeSlot == null || timeSlot.Status != TimeSlotStatus.Available)
                return null;

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId && s.ParentUserId == parentUserId);

            if (student == null)
                return null;

            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.StudentId == dto.StudentId
                    && b.TeacherId == timeSlot.TeacherId
                    && b.Status == BookingStatus.Confirmed);

            if (existingBooking)
                return null;

            var minutesUntilMeeting = (timeSlot.StartDateTime - DateTime.Now).TotalMinutes;

            if (minutesUntilMeeting < 0)
                return null;

            if (minutesUntilMeeting > 60)
                return null;

            var booking = new Booking
            {
                TimeSlotId = dto.TimeSlotId,
                StudentId = dto.StudentId,
                ParentUserId = parentUserId,
                TeacherId = timeSlot.TeacherId,
                ParentNotes = dto.ParentNotes,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);

            timeSlot.Status = TimeSlotStatus.Booked;

            await _context.SaveChangesAsync();
            var teacherUser = await _context.Teachers
           .Include(t => t.User)
           .Where(t => t.Id == timeSlot.TeacherId)
           .Select(t => t.User)
           .FirstOrDefaultAsync();

            if (teacherUser != null)
            {
                var notificationService = _serviceProvider.GetRequiredService<INotificationService>();
                var title = "Nuova Prenotazione";
                var body = $"{student.FirstName} {student.LastName} ha prenotato un colloquio per il {timeSlot.StartDateTime:dd/MM/yyyy} alle {timeSlot.StartDateTime:HH:mm}";
                var data = new Dictionary<string, string>
                {
                     { "type", "new_booking" },
                     { "bookingId", booking.Id.ToString() },
                     { "studentName", $"{student.FirstName} {student.LastName}" }
                };

                await notificationService.SendNotificationAsync(teacherUser.Id, title, body, data);
            }
            return await GetBookingByIdAsync(booking.Id, parentUserId);
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.ParentUserId == userId);

            if (booking == null || booking.Status != BookingStatus.Confirmed)
                return false;

            var minutesUntilMeeting = (booking.TimeSlot.StartDateTime - DateTime.Now).TotalMinutes;
            if (minutesUntilMeeting < 30)
                return false;

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancelledByUserId = userId;

            booking.TimeSlot.Status = TimeSlotStatus.Available;

            await _context.SaveChangesAsync();
            var teacherUser = await _context.Teachers
                .Include(t => t.User)
                .Where(t => t.Id == booking.TeacherId)
                .Select(t => t.User)
                .FirstOrDefaultAsync();

            if (teacherUser != null)
            {
                var notificationService = _serviceProvider.GetRequiredService<INotificationService>();
                var title = "Prenotazione Cancellata";
                var body = $"Un genitore ha cancellato la prenotazione del {booking.TimeSlot.StartDateTime:dd/MM/yyyy} alle {booking.TimeSlot.StartDateTime:HH:mm}";
                var data = new Dictionary<string, string>
                {
                     { "type", "booking_cancelled" },
                     { "bookingId", booking.Id.ToString() }
                };

                await notificationService.SendNotificationAsync(teacherUser.Id, title, body, data);
            }

            await _notificationPreferenceService.NotifySlotBecameAvailableAsync(
                booking.TeacherId,
                booking.TimeSlot.StartDateTime
            );

            return true;
        }
        public async Task<List<TeacherBookingDto>> GetTeacherBookingsAsync(int teacherId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.Student)
                .Include(b => b.ParentUser)
                .Where(b => b.TeacherId == teacherId && b.Status == BookingStatus.Confirmed)
                .OrderBy(b => b.TimeSlot.StartDateTime)
                .Select(b => new TeacherBookingDto
                {
                    Id = b.Id,
                    MeetingDateTime = b.TimeSlot.StartDateTime,
                    DurationMinutes = b.TimeSlot.DurationMinutes,
                    StudentName = b.Student.FirstName + " " + b.Student.LastName,
                    StudentClass = b.Student.Class,
                    ParentName = b.ParentUser.FirstName + " " + b.ParentUser.LastName,
                    ParentEmail = b.ParentUser.Email,
                    ParentPhone = b.ParentUser.Phone,
                    ParentNotes = b.ParentNotes,
                    TeacherNotes = b.TeacherNotes,
                    Status = b.Status.ToString(),
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return bookings;
        }

        public async Task<TeacherBookingDto?> AddTeacherNotesAsync(int bookingId, int teacherId, string notes)
        {
            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .Include(b => b.Student)
                .Include(b => b.ParentUser)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.TeacherId == teacherId);

            if (booking == null)
                return null;

            booking.TeacherNotes = notes;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var notificationService = _serviceProvider.GetRequiredService<INotificationService>();
            var title = "Note Docente Aggiunte";
            var body = $"Il docente ha aggiunto note al colloquio del {booking.TimeSlot.StartDateTime:dd/MM/yyyy}";
            var data = new Dictionary<string, string>
            {
                    { "type", "teacher_notes" },
                    { "bookingId", booking.Id.ToString() }
            };

            await notificationService.SendNotificationAsync(booking.ParentUserId, title, body, data);

            return new TeacherBookingDto
            {
                Id = booking.Id,
                MeetingDateTime = booking.TimeSlot.StartDateTime,
                DurationMinutes = booking.TimeSlot.DurationMinutes,
                StudentName = booking.Student.FirstName + " " + booking.Student.LastName,
                StudentClass = booking.Student.Class,
                ParentName = booking.ParentUser.FirstName + " " + booking.ParentUser.LastName,
                ParentEmail = booking.ParentUser.Email,
                ParentPhone = booking.ParentUser.Phone,
                ParentNotes = booking.ParentNotes,
                TeacherNotes = booking.TeacherNotes,
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt
            };
        }
        public async Task<bool> SkipBookingAsync(int bookingId, int teacherId)
        {
            var booking = await _context.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.TeacherId == teacherId);

            if (booking == null || booking.Status != BookingStatus.Confirmed)
                return false;

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;
            booking.CancelledByUserId = null;
            booking.TeacherNotes = (booking.TeacherNotes ?? "") + " [Saltato dal docente]";

            booking.TimeSlot.Status = Models.Enums.TimeSlotStatus.Available;

            await _context.SaveChangesAsync();

            var notificationService = _serviceProvider.GetRequiredService<INotificationService>();
            var title = "Colloquio Cancellato";
            var body = $"Il docente ha cancellato il colloquio del {booking.TimeSlot.StartDateTime:dd/MM/yyyy} alle {booking.TimeSlot.StartDateTime:HH:mm}";
            var data = new Dictionary<string, string>
            {
                  { "type", "teacher_skipped" },
                  { "bookingId", booking.Id.ToString() }
            };

            await notificationService.SendNotificationAsync(booking.ParentUserId, title, body, data);
            return true;
        }
    }
}