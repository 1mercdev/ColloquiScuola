using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class AddTeacherViewModel : BaseViewModel
    {
        private readonly IAdminService _adminService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string phone = string.Empty;

        [ObservableProperty]
        private string subject = string.Empty;

        [ObservableProperty]
        private string classroom = string.Empty;

        [ObservableProperty]
        private string notes = string.Empty;

        [ObservableProperty]
        private int maxDailyBookings = 0;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public AddTeacherViewModel(IAdminService adminService)
        {
            _adminService = adminService;
            Title = "Aggiungi Docente";
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy)
                return;

            // Validazioni
            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Subject))
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

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var request = new CreateTeacherRequest
                {
                    Email = Email,
                    Password = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone,
                    Subject = Subject,
                    Classroom = string.IsNullOrWhiteSpace(Classroom) ? null : Classroom,
                    Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes,
                    MaxDailyBookings = MaxDailyBookings
                };

                var teacher = await _adminService.CreateTeacherAsync(request);

                if (teacher != null)
                {
                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Docente creato correttamente", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = "Errore durante la creazione. Email già registrata?";
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
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}