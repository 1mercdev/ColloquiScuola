using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using System.Collections.ObjectModel;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class MyBookingsViewModel : BaseViewModel
    {
        private readonly IBookingService _bookingService;

        [ObservableProperty]
        private ObservableCollection<Booking> bookings = new();

        [ObservableProperty]
        private bool hasBookings;

        public MyBookingsViewModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
            Title = "Le Mie Prenotazioni";
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

                var bookingsList = await _bookingService.GetMyBookingsAsync();

                foreach (var booking in bookingsList)
                {
                    Bookings.Add(booking);
                }

                HasBookings = Bookings.Count > 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore",
                    $"Impossibile caricare le prenotazioni: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelBookingAsync(Booking booking)
        {
            if (booking == null)
                return;

            var minutesUntilMeeting = (booking.MeetingDateTime - DateTime.Now).TotalMinutes;

            if (minutesUntilMeeting < 30)
            {
                await Shell.Current.DisplayAlertAsync("Impossibile cancellare",
                    "Puoi cancellare solo se mancano almeno 30 minuti all'inizio del colloquio.", "OK");
                return;
            }

            var confirm = await Shell.Current.DisplayAlertAsync(
                "Conferma Cancellazione",
                $"Vuoi cancellare la prenotazione con {booking.TeacherName} del {booking.MeetingDateFormatted} alle {booking.MeetingTimeFormatted}?",
                "Sì",
                "No");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;
                var success = await _bookingService.CancelBookingAsync(booking.Id);

                if (success)
                {
                    Bookings.Remove(booking);
                    HasBookings = Bookings.Count > 0;
                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Prenotazione cancellata", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Errore",
                        "Impossibile cancellare la prenotazione", "OK");
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
        private async Task ViewBookingDetailsAsync(Booking booking)
        {
            if (booking == null)
                return;

            var details = $"Docente: {booking.TeacherName}\n" +
                          $"Materia: {booking.Subject}\n" +
                          $"Data: {booking.MeetingDateFormatted}\n" +
                          $"Ora: {booking.MeetingTimeFormatted}\n" +
                          $"Durata: {booking.DurationFormatted}\n" +
                          $"Studente: {booking.StudentName} ({booking.StudentClass})\n" +
                          $"Stato: {booking.Status}\n\n" +
                          $"Note Genitore:\n{booking.ParentNotes ?? "Nessuna nota"}";

            if (!string.IsNullOrEmpty(booking.TeacherNotes))
            {
                details += $"\n\nNote Docente:\n{booking.TeacherNotes}";
            }

            await Shell.Current.DisplayAlertAsync("Dettagli Prenotazione", details, "OK");
        }
    }
}