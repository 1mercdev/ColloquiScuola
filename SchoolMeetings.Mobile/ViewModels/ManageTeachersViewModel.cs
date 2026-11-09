using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using System.Collections.ObjectModel;

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class ManageTeachersViewModel : BaseViewModel
    {
        private readonly IAdminService _adminService;

        [ObservableProperty]
        private ObservableCollection<Teacher> teachers = new();

        [ObservableProperty]
        private bool hasTeachers;

        public ManageTeachersViewModel(IAdminService adminService)
        {
            _adminService = adminService;
            Title = "Gestione Docenti";
        }

        [RelayCommand]
        private async Task LoadTeachersAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Teachers.Clear();

                var teachersList = await _adminService.GetAllTeachersAsync();

                foreach (var teacher in teachersList)
                {
                    Teachers.Add(teacher);
                }

                HasTeachers = Teachers.Count > 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore",
                    $"Impossibile caricare i docenti: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddTeacherAsync()
        {
            await Shell.Current.GoToAsync("addteacher");
        }

        [RelayCommand]
        private async Task EditTeacherAsync(Teacher teacher)
        {
            if (teacher == null)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "TeacherId", teacher.Id }
            };

            await Shell.Current.GoToAsync("editteacher", navigationParameter);
        }

        [RelayCommand]
        private async Task DeleteTeacherAsync(Teacher teacher)
        {
            if (teacher == null)
                return;

            var confirm = await Shell.Current.DisplayAlertAsync(
                "Conferma",
                $"Vuoi eliminare il docente {teacher.FullName}? Il docente non potrà essere riattivato in seguito.",
                "Sì",
                "No");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;
                var success = await _adminService.DeleteTeacherAsync(teacher.Id);

                if (success)
                {
                    Teachers.Remove(teacher);
                    HasTeachers = Teachers.Count > 0;
                    await Shell.Current.DisplayAlertAsync("Successo",
                        "Docente disattivato", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Errore",
                        "Impossibile disattivare il docente", "OK");
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