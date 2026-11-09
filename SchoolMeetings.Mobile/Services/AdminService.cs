using SchoolMeetings.Mobile.Helpers;
using SchoolMeetings.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SchoolMeetings.Mobile.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public AdminService(HttpClient httpClient, IAuthService authService)
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

        public async Task<List<Teacher>> GetAllTeachersAsync()
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
                System.Diagnostics.Debug.WriteLine($"GetAllTeachers error: {ex.Message}");
                return new List<Teacher>();
            }
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int teacherId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync($"{Constants.ApiBaseUrl}/api/teachers/{teacherId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Teacher>();
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTeacherById error: {ex.Message}");
                return null;
            }
        }

        public async Task<Teacher?> CreateTeacherAsync(CreateTeacherRequest request)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsJsonAsync(
                    $"{Constants.ApiBaseUrl}/api/teachers",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Teacher>();
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateTeacher error: {ex.Message}");
                return null;
            }
        }

        public async Task<Teacher?> UpdateTeacherAsync(int teacherId, UpdateTeacherRequest request)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PutAsJsonAsync(
                    $"{Constants.ApiBaseUrl}/api/teachers/{teacherId}",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Teacher>();
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateTeacher error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteTeacherAsync(int teacherId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.DeleteAsync(
                    $"{Constants.ApiBaseUrl}/api/teachers/{teacherId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteTeacher error: {ex.Message}");
                return false;
            }
        }

        public async Task<CleanupResult?> CleanupDatabaseAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsync(
                    $"{Constants.ApiBaseUrl}/api/admin/cleanup",
                    null);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CleanupResult>();
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CleanupDatabase error: {ex.Message}");
                return null;
            }
        }
    }
}