namespace SchoolMeetings.API.DTOs.TimeSlots
{
    public class GenerateTimeSlotsResponseDto
    {
        public int SlotsCreated { get; set; }
        public List<TimeSlotDto> Slots { get; set; } = new List<TimeSlotDto>();
    }
}