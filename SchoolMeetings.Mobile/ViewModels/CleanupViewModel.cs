using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Services;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class CleanupViewModel : BaseViewModel
    {
        private readonly IAdminService _adminService;

        [ObservableProperty]
        private string resultMessage = string.Empty;

        [ObservableProperty]
        private bool hasResult;

        public CleanupViewModel(IAdminService adminService)
        {
            _adminService = adminService;
            Title = "Pulizia Database";
        }

        [RelayCommand]
        private async Task CleanupAsync()
        {
            if (IsBusy)
                return;

            var confirm = await Shell.Current.DisplayAlertAsync(
                "Conferma",
                "Vuoi eliminare tutte le prenotazioni completate/cancellate e gli slot passati?\n\nQuesta operazione non può essere annullata.",
                "Sì, procedi",
                "Annulla");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;
                ResultMessage = string.Empty;
                HasResult = false;

                var result = await _adminService.CleanupDatabaseAsync();

                if (result != null)
                {
                    ResultMessage = $"✓ Pulizia completata!\n\n" +
                                    $"Prenotazioni eliminate: {result.BookingsDeleted}\n" +
                                    $"Slot eliminati: {result.TimeSlotsDeleted}\n\n" +
                                    $"{result.Message}";
                    HasResult = true;

                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Database pulito correttamente", "OK");
                }
                else
                {
                    ResultMessage = "✗ Errore durante la pulizia del database";
                    HasResult = true;
                }
            }
            catch (Exception ex)
            {
                ResultMessage = $"✗ Errore: {ex.Message}";
                HasResult = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}