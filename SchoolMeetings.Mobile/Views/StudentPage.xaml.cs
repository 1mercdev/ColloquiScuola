using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views;

public partial class StudentsPage : ContentPage
{
    private readonly StudentsViewModel _viewModel;

    public StudentsPage(StudentsViewModel viewModel)
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