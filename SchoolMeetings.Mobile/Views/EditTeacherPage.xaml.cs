using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class EditTeacherPage : ContentPage
    {
        private readonly EditTeacherViewModel _viewModel;

        public EditTeacherPage(EditTeacherViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadTeacherCommand.ExecuteAsync(null);
        }
    }
}