using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;
using SchoolMeetings.Mobile.Helpers;
using SchoolMeetings.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SchoolMeetings.Mobile.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public NotificationService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();

                CrossFirebaseCloudMessaging.Current.NotificationReceived += OnNotificationReceived;
                CrossFirebaseCloudMessaging.Current.NotificationTapped += OnNotificationTapped;

                System.Diagnostics.Debug.WriteLine("Firebase Cloud Messaging initialized");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase initialization error: {ex.Message}");
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                System.Diagnostics.Debug.WriteLine($"FCM Token: {token}");
                return token;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting FCM token: {ex.Message}");
                return null;
            }
        }

        public async Task RegisterTokenAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("No FCM token available");
                    return;
                }

                var authToken = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(authToken))
                {
                    System.Diagnostics.Debug.WriteLine("User not authenticated");
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authToken);

                var request = new RegisterTokenRequest
                {
                    Token = token,
                    Platform = DeviceInfo.Platform == DevicePlatform.Android ? "Android" : "iOS"
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{Constants.ApiBaseUrl}/api/notifications/register-token",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("FCM token registered successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to register FCM token: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registering FCM token: {ex.Message}");
            }
        }

        public async Task UnregisterTokenAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return;

                var authToken = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(authToken))
                    return;

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.DeleteAsync(
                    $"{Constants.ApiBaseUrl}/api/notifications/unregister-token");

                System.Diagnostics.Debug.WriteLine("FCM token unregistered");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error unregistering FCM token: {ex.Message}");
            }
        }

        private void OnNotificationReceived(object? sender, FCMNotificationReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Notification received: {e.Notification.Title}");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlertAsync(
                    e.Notification.Title ?? "Notifica",
                    e.Notification.Body ?? "",
                    "OK");
            });
        }

        private void OnNotificationTapped(object? sender, FCMNotificationTappedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Notification tapped: {e.Notification.Title}");

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (e.Notification.Data?.TryGetValue("type", out var type) == true)
                {
                    HandleNotificationNavigation(type, e.Notification.Data);
                }
            });
        }

        private void HandleNotificationNavigation(string type, IDictionary<string, string> data)
        {
            switch (type)
            {
                case "new_booking":
                case "booking_cancelled":
                    Shell.Current.GoToAsync("///teacherbookings");
                    break;

                case "reminder":
                case "teacher_notes":
                case "teacher_skipped":
                    Shell.Current.GoToAsync("///mybookings");
                    break;
            }
        }
    }
}