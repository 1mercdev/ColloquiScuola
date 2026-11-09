using Microsoft.EntityFrameworkCore;
using SchoolMeetings.API.Models;
using SchoolMeetings.API.Models.Enums;

namespace SchoolMeetings.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for each entity

        public DbSet<School> Schools { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<NotificationToken> NotificationTokens { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<School>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Name);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => new { e.SchoolId, e.Role });

                entity.Property(e => e.Role)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Users_Role",
                    "Role IN ('Admin', 'Teacher', 'Parent')"
                ));

                entity.HasOne(e => e.School)
                    .WithMany(s => s.Users)
                    .HasForeignKey(e => e.SchoolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(e => e.ParentUserId);
                entity.HasIndex(e => e.SchoolId);
                entity.HasIndex(e => new { e.SchoolId, e.Class });

                entity.HasOne(e => e.ParentUser)
                    .WithMany(u => u.Students)
                    .HasForeignKey(e => e.ParentUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.School)
                    .WithMany(s => s.Students)
                    .HasForeignKey(e => e.SchoolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.SchoolId);
                entity.HasIndex(e => new { e.SchoolId, e.Subject });

                entity.HasOne(e => e.User)
                    .WithOne(u => u.Teacher)
                    .HasForeignKey<Teacher>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.School)
                    .WithMany(s => s.Teachers)
                    .HasForeignKey(e => e.SchoolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TimeSlot>(entity =>
            {
                entity.HasIndex(e => e.TeacherId);
                entity.HasIndex(e => e.StartDateTime);
                entity.HasIndex(e => new { e.TeacherId, e.StartDateTime });
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_TimeSlots_Status",
                    "Status IN ('Available', 'Booked', 'Blocked')"
                ));

                entity.HasOne(e => e.Teacher)
                    .WithMany(t => t.TimeSlots)
                    .HasForeignKey(e => e.TeacherId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasIndex(e => e.TimeSlotId).IsUnique();
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.ParentUserId);
                entity.HasIndex(e => e.TeacherId);
                entity.HasIndex(e => new { e.StudentId, e.TeacherId, e.Status });
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Bookings_Status",
                    "Status IN ('Confirmed', 'Cancelled', 'Completed')"
                ));

                entity.HasOne(e => e.TimeSlot)
                    .WithOne(ts => ts.Booking)
                    .HasForeignKey<Booking>(e => e.TimeSlotId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ParentUser)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(e => e.ParentUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Teacher)
                    .WithMany(t => t.Bookings)
                    .HasForeignKey(e => e.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CancelledByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CancelledByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<NotificationToken>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Token).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.NotificationTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NotificationPreference>(entity =>
            {
                entity.HasOne(np => np.User)
                .WithMany()
                .HasForeignKey(np => np.UserId);

                entity.HasOne(np => np.Teacher)
                .WithMany()
                .HasForeignKey(np => np.TeacherId);

                entity.HasIndex(np => new { np.UserId, np.TeacherId }).IsUnique();
            });
        }
    }
}