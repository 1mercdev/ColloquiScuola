namespace SchoolMeetings.Mobile.Models
{
    public class GenerateTimeSlotsRequest
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 15;
    }
}