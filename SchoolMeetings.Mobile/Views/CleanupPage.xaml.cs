using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class CleanupPage : ContentPage
    {
        public CleanupPage(CleanupViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}