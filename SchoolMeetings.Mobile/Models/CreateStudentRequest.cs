namespace SchoolMeetings.Mobile.Models
{
    public class CreateStudentRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
}