using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Services;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class LogoutViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string userName = string.Empty;

        public LogoutViewModel(IAuthService authService)
        {
            _authService = authService;
            Title = "Logout";
        }

        [RelayCommand]
        private async Task LoadUserInfoAsync()
        {
            var name = await _authService.GetUserNameAsync();
            UserName = name ?? "Utente";
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("///login");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore", $"Errore durante il logout: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}