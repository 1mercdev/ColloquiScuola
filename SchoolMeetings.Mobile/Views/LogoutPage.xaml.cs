using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views;

public partial class LogoutPage : ContentPage
{
    private readonly LogoutViewModel _viewModel;

    public LogoutPage(LogoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadUserInfoCommand.ExecuteAsync(null);
    }
}