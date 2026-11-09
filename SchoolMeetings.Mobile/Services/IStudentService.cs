using SchoolMeetings.Mobile.Models;

namespace SchoolMeetings.Mobile.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetMyStudentsAsync();
        Task<Student?> CreateStudentAsync(CreateStudentRequest request);
        Task<bool> DeleteStudentAsync(int studentId);
    }
}