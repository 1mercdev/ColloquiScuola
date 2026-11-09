using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class MyBookingsPage : ContentPage
    {
        private readonly MyBookingsViewModel _viewModel;

        public MyBookingsPage(MyBookingsViewModel viewModel)
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