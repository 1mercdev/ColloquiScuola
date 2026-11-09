using SchoolMeetings.API.DTOs.Students;

namespace SchoolMeetings.API.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentDto>> GetMyStudentsAsync(int parentUserId);
        Task<StudentDto?> GetStudentByIdAsync(int studentId, int parentUserId);
        Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, int parentUserId, int schoolId);
        Task<StudentDto?> UpdateStudentAsync(int studentId, UpdateStudentDto dto, int parentUserId);
        Task<bool> DeleteStudentAsync(int studentId, int parentUserId);
    }
}