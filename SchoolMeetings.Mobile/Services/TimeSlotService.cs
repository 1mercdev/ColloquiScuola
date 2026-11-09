using SchoolMeetings.Mobile.Helpers;
using SchoolMeetings.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SchoolMeetings.Mobile.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public TimeSlotService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<TimeSlot>> GetTodaySlotsByTeacherAsync(int teacherId)
        {
            try
            {
                await AddAuthHeaderAsync();

                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var response = await _httpClient.GetAsync(
                    $"{Constants.ApiBaseUrl}/api/timeslots/teacher/{teacherId}?fromDate={today}");

                if (response.IsSuccessStatusCode)
                {
                    var slots = await response.Content.ReadFromJsonAsync<List<TimeSlot>>();

                    var todaySlots = slots?
                        .Where(s => s.StartDateTime.Date == DateTime.Today)
                        .OrderBy(s => s.StartDateTime)
                        .ToList() ?? new List<TimeSlot>();

                    return todaySlots;
                }

                return new List<TimeSlot>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTodaySlotsByTeacher error: {ex.Message}");
                return new List<TimeSlot>();
            }
        }

        public async Task<TimeSlot?> GetSlotByIdAsync(int slotId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync(
                    $"{Constants.ApiBaseUrl}/api/timeslots/{slotId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TimeSlot>();
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetSlotById error: {ex.Message}");
                return null;
            }
        }
    }
}