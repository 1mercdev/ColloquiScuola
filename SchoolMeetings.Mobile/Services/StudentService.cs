using SchoolMeetings.Mobile.Helpers;
using SchoolMeetings.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SchoolMeetings.Mobile.Services
{
    public class StudentService : IStudentService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public StudentService(HttpClient httpClient, IAuthService authService)
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

        public async Task<List<Student>> GetMyStudentsAsync()
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.GetAsync($"{Constants.ApiBaseUrl}/api/students");

                if (response.IsSuccessStatusCode)
                {
                    var students = await response.Content.ReadFromJsonAsync<List<Student>>();
                    return students ?? new List<Student>();
                }

                return new List<Student>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetStudents error: {ex.Message}");
                return new List<Student>();
            }
        }

        public async Task<Student?> CreateStudentAsync(CreateStudentRequest request)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.PostAsJsonAsync($"{Constants.ApiBaseUrl}/api/students", request);

                if (response.IsSuccessStatusCode)
                {
                    var student = await response.Content.ReadFromJsonAsync<Student>();
                    return student;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateStudent error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            try
            {
                await AddAuthHeaderAsync();
                var response = await _httpClient.DeleteAsync($"{Constants.ApiBaseUrl}/api/students/{studentId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteStudent error: {ex.Message}");
                return false;
            }
        }
    }
}