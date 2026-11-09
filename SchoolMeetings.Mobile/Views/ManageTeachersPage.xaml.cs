using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class ManageTeachersPage : ContentPage
    {
        private readonly ManageTeachersViewModel _viewModel;

        public ManageTeachersPage(ManageTeachersViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadTeachersCommand.ExecuteAsync(null);
        }
    }
}