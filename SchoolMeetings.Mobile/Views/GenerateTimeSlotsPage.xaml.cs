using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class GenerateTimeSlotsPage : ContentPage
    {
        public GenerateTimeSlotsPage(GenerateTimeSlotsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}