using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.DTOs.TimeSlots;
using SchoolMeetings.API.Models;
using SchoolMeetings.API.Models.Enums;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationPreferenceService _notificationPreferenceService;

        public TimeSlotService(ApplicationDbContext context, INotificationPreferenceService notificationPreferenceService)
        {
            _context = context;
            _notificationPreferenceService = notificationPreferenceService;
        }

        public async Task<List<TimeSlotDto>> GetAvailableSlotsByTeacherAsync(int teacherId, DateTime? fromDate = null)
        {
            var startDate = fromDate ?? DateTime.Now;

            var slots = await _context.TimeSlots
                .Include(ts => ts.Teacher)
                .ThenInclude(t => t.User)
                .Where(ts => ts.TeacherId == teacherId
                    && ts.StartDateTime >= startDate
                    && ts.Status == TimeSlotStatus.Available)
                .OrderBy(ts => ts.StartDateTime)
                .Select(ts => new TimeSlotDto
                {
                    Id = ts.Id,
                    TeacherId = ts.TeacherId,
                    TeacherName = ts.Teacher.User.FirstName + " " + ts.Teacher.User.LastName,
                    Subject = ts.Teacher.Subject,
                    StartDateTime = ts.StartDateTime,
                    EndDateTime = ts.EndDateTime,
                    DurationMinutes = ts.DurationMinutes,
                    Status = ts.Status.ToString(),
                    IsAvailable = ts.Status == TimeSlotStatus.Available
                })
                .ToListAsync();

            return slots;
        }

        public async Task<TimeSlotDto?> GetSlotByIdAsync(int slotId)
        {
            var slot = await _context.TimeSlots
                .Include(ts => ts.Teacher)
                .ThenInclude(t => t.User)
                .Where(ts => ts.Id == slotId)
                .Select(ts => new TimeSlotDto
                {
                    Id = ts.Id,
                    TeacherId = ts.TeacherId,
                    TeacherName = ts.Teacher.User.FirstName + " " + ts.Teacher.User.LastName,
                    Subject = ts.Teacher.Subject,
                    StartDateTime = ts.StartDateTime,
                    EndDateTime = ts.EndDateTime,
                    DurationMinutes = ts.DurationMinutes,
                    Status = ts.Status.ToString(),
                    IsAvailable = ts.Status == TimeSlotStatus.Available
                })
                .FirstOrDefaultAsync();

            return slot;
        }
        public async Task<GenerateTimeSlotsResponseDto> GenerateSlotsAsync(int teacherId, GenerateTimeSlotsDto dto)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher == null)
                throw new InvalidOperationException("Docente non trovato");

            if (dto.EndTime <= dto.StartTime)
                throw new InvalidOperationException("L'orario di fine deve essere dopo l'orario di inizio");

            var slots = new List<TimeSlot>();
            var startDateTime = dto.Date.ToDateTime(dto.StartTime);
            var endDateTime = dto.Date.ToDateTime(dto.EndTime);

            var currentDateTime = startDateTime;

            while (currentDateTime.AddMinutes(dto.SlotDurationMinutes) <= endDateTime)
            {
                var slot = new TimeSlot
                {
                    TeacherId = teacherId,
                    StartDateTime = currentDateTime,
                    EndDateTime = currentDateTime.AddMinutes(dto.SlotDurationMinutes),
                    DurationMinutes = dto.SlotDurationMinutes,
                    Status = Models.Enums.TimeSlotStatus.Available,
                    CreatedAt = DateTime.UtcNow
                };

                slots.Add(slot);
                currentDateTime = currentDateTime.AddMinutes(dto.SlotDurationMinutes);
            }

            _context.TimeSlots.AddRange(slots);
            await _context.SaveChangesAsync();

            await _notificationPreferenceService.NotifyUsersAboutNewSlotsAsync(teacherId, startDateTime);

            var slotDtos = slots.Select(s => new TimeSlotDto
            {
                Id = s.Id,
                TeacherId = s.TeacherId,
                TeacherName = teacher.User.FirstName + " " + teacher.User.LastName,
                Subject = teacher.Subject,
                Classroom = teacher.Classroom,
                StartDateTime = s.StartDateTime,
                EndDateTime = s.EndDateTime,
                DurationMinutes = s.DurationMinutes,
                Status = s.Status.ToString(),
                IsAvailable = true
            }).ToList();

            return new GenerateTimeSlotsResponseDto
            {
                SlotsCreated = slots.Count,
                Slots = slotDtos
            };
        }
    }
}