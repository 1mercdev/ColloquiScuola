using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class TimeSlotsPage : ContentPage
    {
        private readonly TimeSlotsViewModel _viewModel;

        public TimeSlotsPage(TimeSlotsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Carica gli slot quando la pagina appare
            if (_viewModel.TeacherId > 0)
            {
                await _viewModel.LoadTimeSlotsCommand.ExecuteAsync(null);
            }
        }
    }
}