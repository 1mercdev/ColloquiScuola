using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class TeacherBookingsPage : ContentPage
    {
        private readonly TeacherBookingsViewModel _viewModel;

        public TeacherBookingsPage(TeacherBookingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadBookingsCommand.ExecuteAsync(null);
        }
    }
}