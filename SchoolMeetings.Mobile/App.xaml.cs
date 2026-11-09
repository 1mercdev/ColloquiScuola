using SchoolMeetings.Mobile.Services;

namespace SchoolMeetings.Mobile
{
    public partial class App : Application
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;

        public App(INotificationService notificationService, IAuthService authService)
        {
            InitializeComponent();
            _notificationService = notificationService;
            _authService = authService;

            // Inizializza notifiche
            InitializeNotifications();

            //Application.Current.UserAppTheme = AppTheme.Dark; Nel caso serve cambiare il tema!
        }

        private async void InitializeNotifications()
        {
            try
            {
                await _notificationService.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing notifications: {ex.Message}");
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell(_authService))
            {
                Width = 1280,
                Height = 720,
                MinimumWidth = 800,
                MinimumHeight = 600
            };
        }
    }
}
