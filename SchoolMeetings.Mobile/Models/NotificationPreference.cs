namespace SchoolMeetings.Mobile.Models
{
   public class NotificationPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TeacherId { get; set; }
        public bool NotifyOnSlotAvailable { get; set; }
    }
}
