using SchoolMeetings.API.DTOs.Teacher;
using SchoolMeetings.API.DTOs.Teachers;

namespace SchoolMeetings.API.Services.Interfaces
{
    public interface ITeacherService
    {
        /// <summary>
        /// Ottiene tutti i docenti di una scuola
        /// </summary>
        Task<List<TeacherDto>> GetAllTeachersAsync(int schoolId);

        /// <summary>
        /// Ottiene un docente specifico per ID
        /// </summary>
        Task<TeacherDto?> GetTeacherByIdAsync(int teacherId, int schoolId);
        Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto dto, int schoolId);
        Task<TeacherDto?> UpdateTeacherAsync(int teacherId, UpdateTeacherDto dto, int schoolId);
        Task<bool> DeleteTeacherAsync(int teacherId, int schoolId);
    }
}