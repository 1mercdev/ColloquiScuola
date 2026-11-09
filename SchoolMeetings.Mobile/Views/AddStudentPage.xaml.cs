using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views;

public partial class AddStudentPage : ContentPage
{
    public AddStudentPage(AddStudentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}