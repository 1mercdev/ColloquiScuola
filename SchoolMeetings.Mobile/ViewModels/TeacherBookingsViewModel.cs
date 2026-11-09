using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using System.Collections.ObjectModel;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class TeacherBookingsViewModel : BaseViewModel
    {
        private readonly ITeacherService _teacherService;

        [ObservableProperty]
        private ObservableCollection<TeacherBooking> bookings = new();

        [ObservableProperty]
        private bool hasBookings;

        public TeacherBookingsViewModel(ITeacherService teacherService)
        {
            _teacherService = teacherService;
            Title = "I Miei Colloqui";
        }

        [RelayCommand]
        private async Task LoadBookingsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Bookings.Clear();

                var bookingsList = await _teacherService.GetMyBookingsAsync();

                foreach (var booking in bookingsList)
                {
                    Bookings.Add(booking);
                }

                HasBookings = Bookings.Count > 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore",
                    $"Impossibile caricare i colloqui: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddNotesAsync(TeacherBooking booking)
        {
            if (booking == null)
                return;

            var notes = await Shell.Current.DisplayPromptAsync(
                "Aggiungi Note",
                "Inserisci le tue note sul colloquio:",
                initialValue: booking.TeacherNotes ?? "",
                maxLength: 1000,
                keyboard: Keyboard.Text,
                placeholder: "Note..."
            );

            if (string.IsNullOrWhiteSpace(notes))
                return;

            try
            {
                IsBusy = true;
                var success = await _teacherService.AddNotesAsync(booking.Id, notes);

                if (success)
                {
                    booking.TeacherNotes = notes;
                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Note aggiunte", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Errore",
                        "Impossibile salvare le note", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SkipBookingAsync(TeacherBooking booking)
        {
            if (booking == null)
                return;

            var confirm = await Shell.Current.DisplayAlertAsync(
                "Conferma",
                $"Vuoi saltare/cancellare il colloquio con {booking.StudentName}?",
                "Sì",
                "No");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;
                var success = await _teacherService.SkipBookingAsync(booking.Id);

                if (success)
                {
                    Bookings.Remove(booking);
                    HasBookings = Bookings.Count > 0;
                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Colloquio cancellato", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Errore",
                        "Impossibile cancellare il colloquio", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ViewBookingDetailsAsync(TeacherBooking booking)
        {
            if (booking == null)
                return;

            var details = $"Studente: {booking.StudentInfo}\n" +
                          $"Genitore: {booking.ParentName}\n" +
                          $"Email: {booking.ParentEmail}\n" +
                          $"Telefono: {booking.ParentPhone ?? "N/A"}\n" +
                          $"Data: {booking.MeetingDateFormatted}\n" +
                          $"Ora: {booking.MeetingTimeFormatted}\n" +
                          $"Durata: {booking.DurationFormatted}\n" +
                          $"Stato: {booking.Status}\n\n" +
                          $"Note Genitore:\n{booking.ParentNotes ?? "Nessuna nota"}\n\n" +
                          $"Note Docente:\n{booking.TeacherNotes ?? "Nessuna nota"}";

            await Shell.Current.DisplayAlertAsync("Dettagli Colloquio", details, "OK");
        }
    }
}