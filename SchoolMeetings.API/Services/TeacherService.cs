using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.DTOs.Teacher;
using SchoolMeetings.API.DTOs.Teachers;
using SchoolMeetings.API.Models;
using SchoolMeetings.API.Models.Enums;
using SchoolMeetings.API.Services.Interfaces;

namespace SchoolMeetings.API.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;

        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeacherDto>> GetAllTeachersAsync(int schoolId)
        {
            var teachers = await _context.Teachers
                .Include(t => t.User)
                .Where(t => t.SchoolId == schoolId && t.User.IsActive)
                .OrderBy(t => t.User.LastName)
                .ThenBy(t => t.User.FirstName)
                .Select(t => new TeacherDto
                {
                    Id = t.Id,
                    FirstName = t.User.FirstName,
                    LastName = t.User.LastName,
                    FullName = t.User.FirstName + " " + t.User.LastName,
                    Subject = t.Subject,
                    Classroom = t.Classroom,
                    IsAvailable = t.IsAvailable,
                    Email = t.User.Email,
                    Phone = t.User.Phone
                })
                .ToListAsync();

            return teachers;
        }

        public async Task<TeacherDto?> GetTeacherByIdAsync(int teacherId, int schoolId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .Where(t => t.Id == teacherId && t.SchoolId == schoolId)
                .Select(t => new TeacherDto
                {
                    Id = t.Id,
                    FirstName = t.User.FirstName,
                    LastName = t.User.LastName,
                    FullName = t.User.FirstName + " " + t.User.LastName,
                    Subject = t.Subject,
                    Classroom = t.Classroom,
                    IsAvailable = t.IsAvailable,
                    Email = t.User.Email,
                    Phone = t.User.Phone
                })
                .FirstOrDefaultAsync();

            return teacher;
        }
        public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto dto, int schoolId)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (existingUser)
                throw new InvalidOperationException("Email già registrata");

            var user = new User
            {
                SchoolId = schoolId,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Teacher,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var teacher = new Teacher
            {
                UserId = user.Id,
                SchoolId = schoolId,
                Subject = dto.Subject,
                Classroom = dto.Classroom,
                Notes = dto.Notes,
                MaxDailyBookings = dto.MaxDailyBookings,
                IsAvailable = true
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return new TeacherDto
            {
                Id = teacher.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Subject = teacher.Subject,
                Classroom = teacher.Classroom,
                IsAvailable = teacher.IsAvailable,
                Email = user.Email,
                Phone = user.Phone
            };
        }

        public async Task<TeacherDto?> UpdateTeacherAsync(int teacherId, UpdateTeacherDto dto, int schoolId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == teacherId && t.SchoolId == schoolId);

            if (teacher == null)
                return null;

            teacher.User.FirstName = dto.FirstName;
            teacher.User.LastName = dto.LastName;
            teacher.User.Phone = dto.Phone;

            teacher.Subject = dto.Subject;
            teacher.Classroom = dto.Classroom;
            teacher.Notes = dto.Notes;
            teacher.MaxDailyBookings = dto.MaxDailyBookings;
            teacher.IsAvailable = dto.IsAvailable;

            await _context.SaveChangesAsync();

            return new TeacherDto
            {
                Id = teacher.Id,
                FirstName = teacher.User.FirstName,
                LastName = teacher.User.LastName,
                FullName = teacher.User.FullName,
                Subject = teacher.Subject,
                Classroom = teacher.Classroom,
                IsAvailable = teacher.IsAvailable,
                Email = teacher.User.Email,
                Phone = teacher.User.Phone
            };
        }

        public async Task<bool> DeleteTeacherAsync(int teacherId, int schoolId)
        {
            var teacher = await _context.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == teacherId && t.SchoolId == schoolId);

            if (teacher == null)
                return false;

            teacher.IsAvailable = false;
            teacher.User.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}