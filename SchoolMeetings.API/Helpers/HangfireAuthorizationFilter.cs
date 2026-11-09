using Hangfire.Dashboard;

namespace SchoolMeetings.API.Helpers
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // In development, consenti sempre però da verificare se raggiunge produzione
            return true;
        }
    }
}