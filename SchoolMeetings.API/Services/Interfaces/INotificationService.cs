namespace SchoolMeetings.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(int userId, string title, string body, Dictionary<string, string>? data = null);
        Task<bool> SendNotificationToMultipleAsync(List<int> userIds, string title, string body, Dictionary<string, string>? data = null);
    }
}