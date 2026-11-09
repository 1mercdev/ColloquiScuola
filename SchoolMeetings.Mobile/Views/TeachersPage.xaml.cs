using SchoolMeetings.Mobile.ViewModels;

namespace SchoolMeetings.Mobile.Views;

public partial class TeachersPage : ContentPage
{
    private readonly TeachersViewModel _viewModel;

    public TeachersPage(TeachersViewModel viewModel)
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
    private void OnPageTapped(object sender, EventArgs e)
    {
        this.Unfocus();
    }
}