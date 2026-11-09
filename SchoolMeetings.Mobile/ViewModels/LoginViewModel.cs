using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Services;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            Title = "Login";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Inserisci email e password";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var result = await _authService.LoginAsync(Email, Password);

                if (result.IsSuccess)
                {
                    var role = await _authService.GetUserRoleAsync();

                    if (role == "Parent")
                    {
                        await Shell.Current.GoToAsync("///teachers");
                    }
                    else if (role == "Teacher")
                    {
                        await Shell.Current.GoToAsync("///teacherhome");
                    }
                    else if (role == "Admin")
                    {
                        await Shell.Current.GoToAsync("///adminhome");
                    }
                }
                else
                {
                    ErrorMessage = result.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Errore: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("register");
        }
    }
}