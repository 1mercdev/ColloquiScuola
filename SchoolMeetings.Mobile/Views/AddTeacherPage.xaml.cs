using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views
{
    public partial class AddTeacherPage : ContentPage
    {
        public AddTeacherPage(AddTeacherViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}