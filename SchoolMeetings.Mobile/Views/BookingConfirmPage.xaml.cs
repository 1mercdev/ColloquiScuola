using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class BookingConfirmPage : ContentPage
    {
        private readonly BookingConfirmViewModel _viewModel;

        public BookingConfirmPage(BookingConfirmViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadStudentsCommand.ExecuteAsync(null);
        }
    }
}