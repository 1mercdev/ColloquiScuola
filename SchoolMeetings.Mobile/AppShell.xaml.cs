using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using SchoolMeetings.Mobile.Views;

namespace SchoolMeetings.Mobile;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    public AppShell(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("addstudent", typeof(AddStudentPage));
        Routing.RegisterRoute("timeslots", typeof(TimeSlotsPage));
        Routing.RegisterRoute("bookingconfirm", typeof(BookingConfirmPage));
        Routing.RegisterRoute("addteacher", typeof(AddTeacherPage));
        Routing.RegisterRoute("editteacher", typeof(EditTeacherPage));

        CheckAuthenticationAsync();
    }

    private async void CheckAuthenticationAsync()
    {
        try
        {
            var isAuthenticated = await _authService.IsAuthenticatedAsync();

            if (!isAuthenticated)
            {
                await GoToAsync("///login");
                return;
            }

            // Verifica che il token sia ancora valido
            var isValid = await _authService.ValidateTokenAsync();

            if (!isValid)
            {
                // Token scaduto o non valido, fai logout
                await _authService.LogoutAsync();
                await GoToAsync("///login");
                return;
            }

            // Token valido, naviga in base al ruolo
            var role = await _authService.GetUserRoleAsync();

            if (role == "Parent")
                await GoToAsync("///teachers");
            else if (role == "Teacher")
                await GoToAsync("///teacherhome");
            else if (role == "Admin")
                await GoToAsync("///adminhome");
            else
                await GoToAsync("///login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Auth check error: {ex.Message}");
            await GoToAsync("///login");
        }
    }
}