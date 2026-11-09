using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using System.Collections.ObjectModel;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class StudentsViewModel : BaseViewModel
    {
        private readonly IStudentService _studentService;

        [ObservableProperty]
        private ObservableCollection<Student> students = new();

        [ObservableProperty]
        private bool hasStudents;

        public StudentsViewModel(IStudentService studentService)
        {
            _studentService = studentService;
            Title = "I Miei Figli";
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

                HasStudents = Students.Count > 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore", $"Impossibile caricare i figli: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddStudentAsync()
        {
            await Shell.Current.GoToAsync("addstudent");
        }

        [RelayCommand]
        private async Task DeleteStudentAsync(Student student)
        {
            if (student == null)
                return;

            var confirm = await Shell.Current.DisplayAlertAsync(
                "Conferma",
                $"Vuoi eliminare {student.FullName}?",
                "Sì",
                "No");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;
                var success = await _studentService.DeleteStudentAsync(student.Id);

                if (success)
                {
                    Students.Remove(student);
                    HasStudents = Students.Count > 0;
                    await Shell.Current.DisplayAlertAsync("Successo", "Figlio eliminato", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Errore", "Impossibile eliminare", "OK");
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
    }
}