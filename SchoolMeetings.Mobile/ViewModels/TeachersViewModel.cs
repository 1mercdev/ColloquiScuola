using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolMeetings.Mobile.Models;
using SchoolMeetings.Mobile.Services;
using System.Collections.ObjectModel;

#pragma warning disable CA1416
#pragma warning disable MVVMTK0045

namespace SchoolMeetings.Mobile.ViewModels
{
    public partial class TeachersViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private List<Teacher> _allTeachers = new(); // Lista completa per filtraggio

        [ObservableProperty]
        private ObservableCollection<Teacher> teachers = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        public TeachersViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Docenti Disponibili";
        }
        partial void OnSearchTextChanged(string value)
        {
            FilterTeachers();
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
                _allTeachers.Clear();

                var teachersList = await _apiService.GetTeachersAsync();
                _allTeachers = teachersList;

                FilterTeachers();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore", $"Impossibile caricare i docenti: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FilterTeachers()
        {
            Teachers.Clear();

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allTeachers
                : _allTeachers.Where(t =>
                    t.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Subject.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                ).ToList();

            foreach (var teacher in filtered)
            {
                Teachers.Add(teacher);
            }
        }

        [RelayCommand]
        private async Task TeacherSelectedAsync(Teacher teacher)
        {
            if (teacher == null)
                return;

            var navigationParameter = new Dictionary<string, object?>
            {
                { "TeacherId", teacher.Id },
                { "TeacherName", teacher.FullName },
                { "Subject", teacher.Subject },
                { "Classroom", teacher.Classroom }
            };

            await Shell.Current.GoToAsync("timeslots", navigationParameter);
        }
    }
}