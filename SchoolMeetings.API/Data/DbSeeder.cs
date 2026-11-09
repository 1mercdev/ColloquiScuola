
using SchoolMeetings.API.Models;
using SchoolMeetings.API.Models.Enums;
using BCrypt.Net;

namespace SchoolMeetings.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            //verifica la presenza di dati 
            if (context.Schools.Any())
            {
                return; 
            }

            var school = new School
            {
                Name = "Istituto Comprensivo Demo",
                Code = "DEMO-2025",
                Address = "Via Roma 123",
                City = "Roma",
                Phone = "06-12345678",
                Email = "info@demo.school",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Schools.Add(school);
            await context.SaveChangesAsync();     

            var admin = new User
            {
                SchoolId = school.Id,
                Email = "admin@demo.school",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                FirstName = "Mario",
                LastName = "Amministratore",
                Phone = "333-1111111",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(admin);

            var teacherUser1 = new User
            {
                SchoolId = school.Id,
                Email = "rossi@demo.school",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"),
                Role = UserRole.Teacher,
                FirstName = "Giuseppe",
                LastName = "Rossi",
                Phone = "333-2222222",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(teacherUser1);

            var teacherUser2 = new User
            {
                SchoolId = school.Id,
                Email = "bianchi@demo.school",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"),
                Role = UserRole.Teacher,
                FirstName = "Laura",
                LastName = "Bianchi",
                Phone = "333-3333333",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(teacherUser2);

            var parentUser = new User
            {
                SchoolId = school.Id,
                Email = "genitore@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Parent123!"),
                Role = UserRole.Parent,
                FirstName = "Anna",
                LastName = "Verdi",
                Phone = "333-4444444",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(parentUser);

            await context.SaveChangesAsync();   

            var teacher1 = new Teacher
            {
                UserId = teacherUser1.Id,
                SchoolId = school.Id,
                Subject = "Matematica",
                Classroom = "Aula 12",
                SlotDurationMinutes = 15,
                MaxDailyBookings = 10,
                IsAvailable = true
            };
            context.Teachers.Add(teacher1);

            var teacher2 = new Teacher
            {
                UserId = teacherUser2.Id,
                SchoolId = school.Id,
                Subject = "Italiano",
                Classroom = "Aula 8",
                SlotDurationMinutes = 20,
                MaxDailyBookings = 8,
                IsAvailable = true
            };
            context.Teachers.Add(teacher2);

            await context.SaveChangesAsync();   

            var student = new Student
            {
                ParentUserId = parentUser.Id,
                SchoolId = school.Id,
                FirstName = "Luca",
                LastName = "Verdi",
                Class = "3A",
                Notes = "Studente bravo e motivato",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Students.Add(student);

            await context.SaveChangesAsync();   

            Console.WriteLine(" Database popolato con successo!");
            Console.WriteLine(" Admin: admin@demo.school / Admin123!");
            Console.WriteLine(" Docente 1: rossi@demo.school / Teacher123!");
            Console.WriteLine(" Docente 2: bianchi@demo.school / Teacher123!");
            Console.WriteLine(" Genitore: genitore@email.com / Parent123!");
        }
    }
}