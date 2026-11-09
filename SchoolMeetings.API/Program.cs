using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.Helpers;
using SchoolMeetings.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5090");

//servizi

// JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

// Hangfire (Background Jobs)
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

// Background Jobs
builder.Services.AddScoped<SchoolMeetings.API.BackgroundJobs.CompleteBookingsJob>();

// Authentication JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization();

//Firebase
builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("Firebase"));
builder.Services.AddHostedService<BookingReminderService>();

// Services
builder.Services.AddScoped<SchoolMeetings.API.Services.Interfaces.IAuthService, SchoolMeetings.API.Services.AuthService>();
builder.Services.AddScoped<SchoolMeetings.API.Services.Interfaces.ITeacherService, SchoolMeetings.API.Services.TeacherService>();
builder.Services.AddScoped<SchoolMeetings.API.Services.Interfaces.IStudentService, SchoolMeetings.API.Services.StudentService>();
builder.Services.AddScoped<SchoolMeetings.API.Services.Interfaces.ITimeSlotService, SchoolMeetings.API.Services.TimeSlotService>();
builder.Services.AddScoped<SchoolMeetings.API.Services.Interfaces.IBookingService, SchoolMeetings.API.Services.BookingService>();
builder.Services.AddScoped<SchoolMeetings.API.Services.Interfaces.IAdminService, SchoolMeetings.API.Services.AdminService>();
builder.Services.AddScoped<SchoolMeetings.API.Services.Interfaces.INotificationService, SchoolMeetings.API.Services.NotificationService>();
builder.Services.AddScoped<SchoolMeetings.API.Services.Interfaces.INotificationPreferenceService, SchoolMeetings.API.Services.NotificationPreferenceService>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Config
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Inserisci il token JWT in questo formato: {token}"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        }
    );

});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.WithOrigins("http://localhost", "https://localhost")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();


// Devtools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowMobileApp");

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// RecurringJob
RecurringJob.AddOrUpdate<SchoolMeetings.API.BackgroundJobs.CompleteBookingsJob>(
    "complete-past-bookings",
    job => job.Run(),
    Cron.MinuteInterval(5) // Ogni 5 minuti
);

app.MapControllers();

// Seed del database (solo ambiente dev)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SchoolMeetings.API.Data.DbSeeder.SeedAsync(context);
    }
}

app.Run();