using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SchoolMeetings.Mobile.Services;
using SchoolMeetings.Mobile.ViewModels;
using SchoolMeetings.Mobile.Views;
using SchoolMeetings.Mobile.Helpers;
using Plugin.Firebase.CloudMessaging;
#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace SchoolMeetings.Mobile;

public static class MauiProgram
{
#pragma warning disable CA1416 // must validate platform compatibility (versionchecks)
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        builder.ConfigureLifecycleEvents(events => {
#if ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) =>
            {
                CreateNotificationChannel(activity);
                CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            }));
#elif IOS
/*           events.AddiOS(iOS => iOS.WillFinishLaunching((_,__) => {
                CrossFirebase.Initialize();
                return false;
            }));*/
#endif
        });

#if ANDROID
        static void CreateNotificationChannel(Android.App.Activity activity)
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                var channel = new Android.App.NotificationChannel(
                    "default",
                    "Notifiche ColloquisCuola",
                    Android.App.NotificationImportance.High)
                {
                    Description = "Notifiche per colloqui scolastici"
                };

                var notificationManager = activity.GetSystemService(Android.Content.Context.NotificationService) as Android.App.NotificationManager;
                notificationManager?.CreateNotificationChannel(channel);
            }
        }
#endif

        // HttpClient
#if DEBUG && (ANDROID || IOS)
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
#else
        var handler = new HttpClientHandler();
#endif

        builder.Services.AddHttpClient("API", client =>
        {
            client.BaseAddress = new Uri(Constants.ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .ConfigurePrimaryHttpMessageHandler(() => handler);

        builder.Services.AddSingleton(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

        // Services
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IStudentService, StudentService>();
        builder.Services.AddSingleton<ITimeSlotService, TimeSlotService>();
        builder.Services.AddSingleton<IBookingService, BookingService>();
        builder.Services.AddSingleton<ITeacherService, TeacherService>();
        builder.Services.AddSingleton<IAdminService, AdminService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<TeachersViewModel>();
        builder.Services.AddTransient<StudentsViewModel>();
        builder.Services.AddTransient<AddStudentViewModel>();
        builder.Services.AddTransient<LogoutViewModel>();
        builder.Services.AddTransient<TimeSlotsViewModel>();
        builder.Services.AddTransient<BookingConfirmViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<MyBookingsViewModel>();
        builder.Services.AddTransient<TeacherBookingsViewModel>();
        builder.Services.AddTransient<GenerateTimeSlotsViewModel>();
        builder.Services.AddTransient<ManageTeachersViewModel>();
        builder.Services.AddTransient<AddTeacherViewModel>();
        builder.Services.AddTransient<EditTeacherViewModel>();
        builder.Services.AddTransient<CleanupViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TeachersPage>();
        builder.Services.AddTransient<StudentsPage>();
        builder.Services.AddTransient<AddStudentPage>();
        builder.Services.AddTransient<LogoutPage>();
        builder.Services.AddTransient<TimeSlotsPage>();
        builder.Services.AddTransient<BookingConfirmPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<MyBookingsPage>();
        builder.Services.AddTransient<TeacherBookingsPage>();
        builder.Services.AddTransient<GenerateTimeSlotsPage>();
        builder.Services.AddTransient<ManageTeachersPage>();
        builder.Services.AddTransient<AddTeacherPage>();
        builder.Services.AddTransient<EditTeacherPage>();
        builder.Services.AddTransient<CleanupPage>();

        // Shell
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}