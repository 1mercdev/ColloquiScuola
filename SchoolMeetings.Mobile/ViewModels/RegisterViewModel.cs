using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string phone = string.Empty;

        [ObservableProperty]
        private string schoolCode = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;
        
        [ObservableProperty]
        private bool isAdmin = false;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
            Title = "Registrazione";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword) ||
                string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(SchoolCode))
            {
                ErrorMessage = "Compila tutti i campi obbligatori";
                return;
            }

            if (!Email.Contains("@"))
            {
                ErrorMessage = "Email non valida";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "La password deve essere di almeno 6 caratteri";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Le password non coincidono";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var request = new RegisterRequest
                {
                    Email = Email,
                    Password = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone,
                    SchoolCode = SchoolCode
                };

                var result = IsAdmin ? await _authService.RegisterAdminAsync(request) : await _authService.RegisterAsync(request);

                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Registrazione completata!", "OK");

                    if (IsAdmin)
                    {
                        await Shell.Current.GoToAsync("///manageteachers");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("///teachers");
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
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("///login");
        }
    }
}