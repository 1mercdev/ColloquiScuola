using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class AddStudentViewModel : BaseViewModel
    {
        private readonly IStudentService _studentService;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string studentClass = string.Empty;

        [ObservableProperty]
        private string notes = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public AddStudentViewModel(IStudentService studentService)
        {
            _studentService = studentService;
            Title = "Aggiungi Figlio";
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(StudentClass))
            {
                ErrorMessage = "Compila tutti i campi obbligatori";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var request = new CreateStudentRequest
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Class = StudentClass,
                    Notes = Notes
                };

                var student = await _studentService.CreateStudentAsync(request);

                if (student != null)
                {
                    await Shell.Current.DisplayAlertAsync("Successo", "Figlio aggiunto correttamente", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = "Errore durante il salvataggio";
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