namespace SchoolMeetings.Mobile.Models
{
    public class CreateBookingRequest
    {
        public int TimeSlotId { get; set; }
        public int StudentId { get; set; }
        public string? ParentNotes { get; set; }
    }
}