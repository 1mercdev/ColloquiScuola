using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using System.Collections.ObjectModel;

namespace SchoolMeetings.Mobile.ViewModels
{
    [QueryProperty(nameof(TeacherId), "TeacherId")]
    [QueryProperty(nameof(TeacherName), "TeacherName")]
    [QueryProperty(nameof(Subject), "Subject")]
    [QueryProperty(nameof(Classroom), "Classroom")]
    public partial class TimeSlotsViewModel : BaseViewModel
    {
        private readonly ITimeSlotService _timeSlotService;
        private readonly IApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<TimeSlot> timeSlots = new();

        [ObservableProperty]
        private int teacherId;

        [ObservableProperty]
        private string teacherName = string.Empty;

        [ObservableProperty]
        private string subject = string.Empty;

        [ObservableProperty]
        private string classroom = string.Empty;

        [ObservableProperty]
        private string teacherInfo = string.Empty;

        [ObservableProperty]
        private string classroomInfo = string.Empty;

        [ObservableProperty]
        private string todayDate = string.Empty;

        [ObservableProperty]
        private bool hasSlots;

        [ObservableProperty]
        private bool notifyOnNewSlots;

        [ObservableProperty]
        private bool isLoadingPreference;

        public TimeSlotsViewModel(ITimeSlotService timeSlotService, IApiService apiService)
        {
            _timeSlotService = timeSlotService;
            _apiService = apiService;
            Title = "Orari Disponibili";
            TodayDate = DateTime.Today.ToString("dddd dd MMMM yyyy");
        }

        partial void OnTeacherIdChanged(int value)
        {
            UpdateTeacherInfo();
        }

        partial void OnTeacherNameChanged(string value)
        {
            UpdateTeacherInfo();
        }

        partial void OnSubjectChanged(string value)
        {
            UpdateTeacherInfo();
        }

        partial void OnClassroomChanged(string value)
        {
            UpdateTeacherInfo();
        }

        private void UpdateTeacherInfo()
        {
            if (!string.IsNullOrEmpty(TeacherName) && !string.IsNullOrEmpty(Subject))
            {
                TeacherInfo = $"{TeacherName} - {Subject}";
            }

            if (!string.IsNullOrEmpty(Classroom))
            {
                ClassroomInfo = $" (Aula: {Classroom})";
            }
        }

        [RelayCommand]
        private async Task LoadTimeSlotsAsync()
        {
            if (IsBusy || TeacherId == 0)
                return;

            try
            {
                IsBusy = true;
                TimeSlots.Clear();

                var slots = await _timeSlotService.GetTodaySlotsByTeacherAsync(TeacherId);

                foreach (var slot in slots)
                {
                    TimeSlots.Add(slot);
                }

                HasSlots = TimeSlots.Count > 0;

                await LoadNotificationPreferenceAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore",
                    $"Impossibile caricare gli orari: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadNotificationPreferenceAsync()
        {
            try
            {
                IsLoadingPreference = true;
                NotifyOnNewSlots = await _apiService.GetNotificationPreferenceAsync(TeacherId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading preference: {ex.Message}");
            }
            finally
            {
                IsLoadingPreference = false;
            }
        }

        async partial void OnNotifyOnNewSlotsChanged(bool value)
        {
            if (IsLoadingPreference) return; 

            try
            {
                var success = await _apiService.SetNotificationPreferenceAsync(TeacherId, value);
                if (!success)
                {
                    NotifyOnNewSlots = !value;
                    await Shell.Current.DisplayAlertAsync("Errore", "Impossibile salvare la preferenza", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving preference: {ex.Message}");
                NotifyOnNewSlots = !value;
            }
        }

        [RelayCommand]
        private async Task SlotSelectedAsync(TimeSlot slot)
        {
            if (slot == null || TeacherId == 0)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "TimeSlotId", slot.Id },
                { "TeacherId", TeacherId },
                { "TeacherName", TeacherName },
                { "Subject", Subject },
                { "StartDateTime", slot.StartDateTime.ToString("O") }, 
                { "EndDateTime", slot.EndDateTime.ToString("O") },
                { "DurationMinutes", slot.DurationMinutes }
            };

            await Shell.Current.GoToAsync("bookingconfirm", navigationParameter);
        }
    }
}