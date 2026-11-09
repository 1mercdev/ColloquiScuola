using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SchoolMeetings.Mobile.ViewModels
{
    [QueryProperty(nameof(TimeSlotId), "TimeSlotId")]
    [QueryProperty(nameof(TeacherId), "TeacherId")]
    [QueryProperty(nameof(TeacherName), "TeacherName")]
    [QueryProperty(nameof(Subject), "Subject")]
    [QueryProperty(nameof(StartDateTime), "StartDateTime")]
    [QueryProperty(nameof(EndDateTime), "EndDateTime")]
    [QueryProperty(nameof(DurationMinutes), "DurationMinutes")]
    public partial class BookingConfirmViewModel : BaseViewModel
    {
        private readonly IBookingService _bookingService;
        private readonly IStudentService _studentService;

        [ObservableProperty]
        private int timeSlotId;

        [ObservableProperty]
        private int teacherId;

        [ObservableProperty]
        private string teacherName = string.Empty;

        [ObservableProperty]
        private string subject = string.Empty;

        [ObservableProperty]
        private string startDateTime = string.Empty;

        [ObservableProperty]
        private string endDateTime = string.Empty;

        [ObservableProperty]
        private int durationMinutes;

        [ObservableProperty]
        private ObservableCollection<Student> students = new();

        [ObservableProperty]
        private Student? selectedStudent;

        [ObservableProperty]
        private string parentNotes = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string meetingInfo = string.Empty;

        public BookingConfirmViewModel(
            IBookingService bookingService,
            IStudentService studentService)
        {
            _bookingService = bookingService;
            _studentService = studentService;
            Title = "Conferma Prenotazione";
        }

        partial void OnStartDateTimeChanged(string value)
        {
            UpdateMeetingInfo();
        }

        partial void OnTeacherNameChanged(string value)
        {
            UpdateMeetingInfo();
        }

        partial void OnSubjectChanged(string value)
        {
            UpdateMeetingInfo();
        }

        private void UpdateMeetingInfo()
        {
            if (!string.IsNullOrEmpty(StartDateTime) &&
                !string.IsNullOrEmpty(EndDateTime) &&
                !string.IsNullOrEmpty(TeacherName))
            {
                try
                {
                    var start = DateTime.Parse(StartDateTime, null, DateTimeStyles.RoundtripKind);
                    var end = DateTime.Parse(EndDateTime, null, DateTimeStyles.RoundtripKind);

                    MeetingInfo = $"{start:dddd dd MMMM yyyy}\n" +
                                  $"{start:HH:mm} - {end:HH:mm} ({DurationMinutes} minuti)\n" +
                                  $"{TeacherName} - {Subject}";
                }
                catch
                {
                    MeetingInfo = $"{TeacherName} - {Subject}";
                }
            }
        }

        [RelayCommand]
        private async Task LoadStudentsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Students.Clear();

                var studentsList = await _studentService.GetMyStudentsAsync();

                foreach (var student in studentsList)
                {
                    Students.Add(student);
                }

                UpdateMeetingInfo();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore",
                    $"Impossibile caricare i figli: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ConfirmBookingAsync()
        {
            if (IsBusy)
                return;

            if (SelectedStudent == null)
            {
                ErrorMessage = "Seleziona uno studente";
                return;
            }

            if (TimeSlotId == 0)
            {
                ErrorMessage = "Slot non valido";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var request = new CreateBookingRequest
                {
                    TimeSlotId = TimeSlotId,
                    StudentId = SelectedStudent.Id,
                    ParentNotes = string.IsNullOrWhiteSpace(ParentNotes) ? null : ParentNotes
                };

                var booking = await _bookingService.CreateBookingAsync(request);

                if (booking != null)
                {
                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Prenotazione confermata!", "OK");

                    await Shell.Current.GoToAsync("///teachers");
                }
                else
                {
                    ErrorMessage = "Impossibile creare la prenotazione. " +
                                   "Verifica che lo slot sia ancora disponibile e che non ci siano già " +
                                   "prenotazioni per questo studente con questo docente.";
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