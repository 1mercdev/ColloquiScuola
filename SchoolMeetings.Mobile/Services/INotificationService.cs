namespace SchoolMeetings.Mobile.Services
{
    public interface INotificationService
    {
        Task InitializeAsync();
        Task<string?> GetTokenAsync();
        Task RegisterTokenAsync();
        Task UnregisterTokenAsync();
    }
}