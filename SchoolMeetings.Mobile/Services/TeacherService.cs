using SchoolMeetings.Mobile.Helpers;
using SchoolMeetings.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SchoolMeetings.Mobile.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public TeacherService(HttpClient httpClient, IAuthService authService)
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

        public async Task<bool> GenerateTimeSlotsAsync(GenerateTimeSlotsRequest request)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsJsonAsync(
                    $"{Constants.ApiBaseUrl}/api/timeslots/generate",
                    request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GenerateTimeSlots error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<TeacherBooking>> GetMyBookingsAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync(
                    $"{Constants.ApiBaseUrl}/api/teacher/bookings");

                if (response.IsSuccessStatusCode)
                {
                    var bookings = await response.Content.ReadFromJsonAsync<List<TeacherBooking>>();
                    return bookings ?? new List<TeacherBooking>();
                }

                return new List<TeacherBooking>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetMyBookings error: {ex.Message}");
                return new List<TeacherBooking>();
            }
        }

        public async Task<bool> AddNotesAsync(int bookingId, string notes)
        {
            try
            {
                await AddAuthHeaderAsync();

                var request = new { Notes = notes };
                var response = await _httpClient.PatchAsJsonAsync(
                    $"{Constants.ApiBaseUrl}/api/teacher/bookings/{bookingId}/notes",
                    request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddNotes error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SkipBookingAsync(int bookingId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsync(
                    $"{Constants.ApiBaseUrl}/api/teacher/bookings/{bookingId}/skip",
                    null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SkipBooking error: {ex.Message}");
                return false;
            }
        }
    }
}