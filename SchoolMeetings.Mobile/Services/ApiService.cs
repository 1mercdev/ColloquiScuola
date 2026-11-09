using SchoolMeetings.Mobile.Helpers;
using SchoolMeetings.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SchoolMeetings.Mobile.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public ApiService(HttpClient httpClient, IAuthService authService)
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

        public async Task<List<Teacher>> GetTeachersAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync($"{Constants.ApiBaseUrl}/api/teachers");

                if (response.IsSuccessStatusCode)
                {
                    var teachers = await response.Content.ReadFromJsonAsync<List<Teacher>>();
                    return teachers ?? new List<Teacher>();
                }

                return new List<Teacher>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTeachers error: {ex.Message}");
                return new List<Teacher>();
            }
        }
        public async Task<bool> GetNotificationPreferenceAsync(int teacherId)
        {
            var response = await _httpClient.GetAsync($"{Constants.ApiBaseUrl}/api/notificationpreferences/teacher/{teacherId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }

        public async Task<bool> SetNotificationPreferenceAsync(int teacherId, bool enabled)
        {
            var request = new SetPreferenceRequest { TeacherId = teacherId, Enabled = enabled };
            var response = await _httpClient.PostAsJsonAsync($"{Constants.ApiBaseUrl}/api/notificationpreferences/setnotificationpreference", request);
            return response.IsSuccessStatusCode;
        }
    }
}