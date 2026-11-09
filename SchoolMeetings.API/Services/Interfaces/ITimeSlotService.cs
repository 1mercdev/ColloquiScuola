using SchoolMeetings.API.DTOs.TimeSlots;

namespace SchoolMeetings.API.Services.Interfaces
{
    public interface ITimeSlotService
    {
        Task<List<TimeSlotDto>> GetAvailableSlotsByTeacherAsync(int teacherId, DateTime? fromDate = null);
        Task<TimeSlotDto?> GetSlotByIdAsync(int slotId);
        Task<GenerateTimeSlotsResponseDto> GenerateSlotsAsync(int teacherId, GenerateTimeSlotsDto dto);
    }
}