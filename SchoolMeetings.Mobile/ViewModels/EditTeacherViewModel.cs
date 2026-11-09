using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;

#pragma warning disable MVVMTK0045
namespace SchoolMeetings.Mobile.ViewModels
{
    [QueryProperty(nameof(TeacherId), "TeacherId")]
    public partial class EditTeacherViewModel : BaseViewModel
    {
        private readonly IAdminService _adminService;

        [ObservableProperty]
        private int teacherId;

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
        private bool isAvailable = true;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public EditTeacherViewModel(IAdminService adminService)
        {
            _adminService = adminService;
            Title = "Modifica Docente";
        }

        [RelayCommand]
        private async Task LoadTeacherAsync()
        {
            if (IsBusy || TeacherId == 0)
                return;

            try
            {
                IsBusy = true;

                var teacher = await _adminService.GetTeacherByIdAsync(TeacherId);

                if (teacher != null)
                {
                    FirstName = teacher.FirstName;
                    LastName = teacher.LastName;
                    Phone = teacher.Phone ?? string.Empty;
                    Subject = teacher.Subject;
                    Classroom = teacher.Classroom ?? string.Empty;
                    IsAvailable = teacher.IsAvailable;
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Errore",
                        "Docente non trovato", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore",
                    $"Impossibile caricare il docente: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy)
                return;

            // validazioni
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Subject))
            {
                ErrorMessage = "Compila tutti i campi obbligatori";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var request = new UpdateTeacherRequest
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone,
                    Subject = Subject,
                    Classroom = string.IsNullOrWhiteSpace(Classroom) ? null : Classroom,
                    Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes,
                    MaxDailyBookings = MaxDailyBookings,
                    IsAvailable = IsAvailable
                };

                var teacher = await _adminService.UpdateTeacherAsync(TeacherId, request);

                if (teacher != null)
                {
                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Docente aggiornato correttamente", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = "Errore durante l'aggiornamento";
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