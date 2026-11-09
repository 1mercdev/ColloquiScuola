using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using System.Collections.ObjectModel;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class GenerateTimeSlotsViewModel : BaseViewModel
    {
        private readonly ITeacherService _teacherService;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private TimeSpan startTime = new TimeSpan(9, 0, 0); // 09:00

        [ObservableProperty]
        private TimeSpan endTime = new TimeSpan(17, 0, 0); // 17:00

        [ObservableProperty]
        private int selectedDuration = 15;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private int estimatedSlots;

        public ObservableCollection<int> SlotDurations { get; } = new(Enumerable.Range(1, 60));

        public GenerateTimeSlotsViewModel(ITeacherService teacherService)
        {
            _teacherService = teacherService;
            Title = "Genera TimeSlots";
            CalculateEstimatedSlots();
        }

        // Ricalcola slot stimati quando cambiano i parametri
        partial void OnStartTimeChanged(TimeSpan value)
        {
            CalculateEstimatedSlots();
        }

        partial void OnEndTimeChanged(TimeSpan value)
        {
            CalculateEstimatedSlots();
        }

        partial void OnSelectedDurationChanged(int value)
        {
            CalculateEstimatedSlots();
        }

        private void CalculateEstimatedSlots()
        {
            if (EndTime > StartTime && SelectedDuration > 0)
            {
                var totalMinutes = (EndTime - StartTime).TotalMinutes;
                EstimatedSlots = (int)(totalMinutes / SelectedDuration);
            }
            else
            {
                EstimatedSlots = 0;
            }
        }

        [RelayCommand]
        private async Task GenerateSlotsAsync()
        {
            if (IsBusy)
                return;

            // Validazioni
            if (SelectedDate < DateTime.Today)
            {
                ErrorMessage = "Non puoi generare slot per date passate";
                return;
            }

            if (EndTime <= StartTime)
            {
                ErrorMessage = "L'orario di fine deve essere dopo l'orario di inizio";
                return;
            }

            if (SelectedDuration < 5 || SelectedDuration > 60)
            {
                ErrorMessage = "La durata deve essere tra 5 e 60 minuti";
                return;
            }

            if (EstimatedSlots == 0)
            {
                ErrorMessage = "Nessuno slot verrà generato con questi parametri";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var request = new GenerateTimeSlotsRequest
                {
                    Date = DateOnly.FromDateTime(SelectedDate),
                    StartTime = TimeOnly.FromTimeSpan(StartTime),
                    EndTime = TimeOnly.FromTimeSpan(EndTime),
                    SlotDurationMinutes = SelectedDuration
                };

                var success = await _teacherService.GenerateTimeSlotsAsync(request);

                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Successo",
                        $"{EstimatedSlots} slot generati con successo!", "OK");

                    SelectedDate = DateTime.Today;
                    StartTime = new TimeSpan(12, 0, 0);
                    EndTime = new TimeSpan(23, 59, 0);
                    SelectedDuration = 15;
                }
                else
                {
                    ErrorMessage = "Errore durante la generazione degli slot";
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
    }
}