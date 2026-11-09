using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.DTOs.Students;
using SchoolMeetings.API.Models;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentDto>> GetMyStudentsAsync(int parentUserId)
        {
            var students = await _context.Students
                .Where(s => s.ParentUserId == parentUserId && s.IsActive)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    FullName = s.FullName,
                    Class = s.Class,
                    Notes = s.Notes,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            return students;
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int studentId, int parentUserId)
        {
            var student = await _context.Students
                .Where(s => s.Id == studentId && s.ParentUserId == parentUserId)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    FullName = s.FullName,
                    Class = s.Class,
                    Notes = s.Notes,
                    IsActive = s.IsActive
                })
                .FirstOrDefaultAsync();

            return student;
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, int parentUserId, int schoolId)
        {
            var student = new Student
            {
                ParentUserId = parentUserId,
                SchoolId = schoolId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Class = dto.Class,
                Notes = dto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                FullName = student.FullName,
                Class = student.Class,
                Notes = student.Notes,
                IsActive = student.IsActive
            };
        }

        public async Task<StudentDto?> UpdateStudentAsync(int studentId, UpdateStudentDto dto, int parentUserId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == studentId && s.ParentUserId == parentUserId);

            if (student == null)
                return null;

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Class = dto.Class;
            student.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                FullName = student.FullName,
                Class = student.Class,
                Notes = student.Notes,
                IsActive = student.IsActive
            };
        }

        public async Task<bool> DeleteStudentAsync(int studentId, int parentUserId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == studentId && s.ParentUserId == parentUserId);

            if (student == null)
                return false;

            student.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}