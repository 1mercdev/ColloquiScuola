namespace SchoolMeetings.API.DTOs.Notifications
{
    public class SetNotificationPreferenceDto
    {
        public int TeacherId { get; set; }
        public bool Enabled { get; set; }
    }
}
