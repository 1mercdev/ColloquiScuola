using SchoolMeetings.Mobile.Helpers;
using SchoolMeetings.Mobile.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SchoolMeetings.Mobile.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;

        public AuthService(HttpClient httpClient, IServiceProvider serviceProvider)
        {
            _httpClient = httpClient;
            _serviceProvider = serviceProvider;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var request = new LoginRequest { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync($"{Constants.ApiBaseUrl}/api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (result != null)
                    {
                        await SecureStorage.SetAsync(Constants.TokenKey, result.Token);
                        await SecureStorage.SetAsync(Constants.UserIdKey, result.User.Id.ToString());
                        await SecureStorage.SetAsync(Constants.UserRoleKey, result.User.Role);
                        await SecureStorage.SetAsync(Constants.SchoolIdKey, result.User.SchoolId.ToString());
                        await SecureStorage.SetAsync(Constants.UserNameKey, result.User.FullName);
                        await SecureStorage.SetAsync(Constants.UserEmailKey, result.User.Email);

                        var notificationService = _serviceProvider.GetService<INotificationService>();
                        if (notificationService != null)
                        {
                            _ = Task.Run(async () => await notificationService.RegisterTokenAsync());
                        }

                        var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                        if (status != PermissionStatus.Granted)
                        {
                            System.Diagnostics.Debug.WriteLine("Notification permission denied");
                        }
                        return AuthResult.Success();
                    }
                }
                var errorMessage = response.StatusCode switch 
                {
                    HttpStatusCode.Unauthorized => "Email o password non corretti",
                    HttpStatusCode.BadRequest => "Dati non validi. Controlla email e password",
                    HttpStatusCode.InternalServerError => "Errore del server. Riprova tra poco",
                    HttpStatusCode.BadGateway => "Errore di comunicazione con il server. Riprova più tardi",
                    HttpStatusCode.ServiceUnavailable => "Servizio temporaneamente non disponibile",
                    HttpStatusCode.RequestTimeout => "Richiesta scaduta. Controlla la connessione",
                    _ when ((int)response.StatusCode >= 500) => "Errore del server. Riprova più tardi",
                    _ => "Errore di connessione. Riprova"
                };
                return AuthResult.Failure(errorMessage);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return AuthResult.Failure("Impossibile connettersi al server. Controlla la connessione internet");
            }
            catch (TaskCanceledException)
            {
                return AuthResult.Failure("Richiesta scaduta. Controlla la connessione");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return AuthResult.Failure("Si è verificato un errore imprevisto");
            }
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{Constants.ApiBaseUrl}/api/auth/register", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (result != null)
                    {
                        await SecureStorage.SetAsync(Constants.TokenKey, result.Token);
                        await SecureStorage.SetAsync(Constants.UserIdKey, result.User.Id.ToString());
                        await SecureStorage.SetAsync(Constants.UserRoleKey, result.User.Role);
                        await SecureStorage.SetAsync(Constants.SchoolIdKey, result.User.SchoolId.ToString());
                        await SecureStorage.SetAsync(Constants.UserNameKey, result.User.FullName);
                        await SecureStorage.SetAsync(Constants.UserEmailKey, result.User.Email);

                        var notificationService = _serviceProvider.GetService<INotificationService>();
                        if (notificationService != null)
                        {
                            _ = Task.Run(async () => await notificationService.RegisterTokenAsync());
                        }

                        return AuthResult.Success();
                    }
                }
                var errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "401 Unauthorized",
                    HttpStatusCode.BadRequest => "Dati non validi. Controlla i dati inseriti e riprova.",
                    HttpStatusCode.InternalServerError => "Errore del server. Riprova tra poco",
                    HttpStatusCode.BadGateway => "Errore di comunicazione con il server. Riprova più tardi",
                    HttpStatusCode.ServiceUnavailable => "Servizio temporaneamente non disponibile",
                    HttpStatusCode.RequestTimeout => "Richiesta scaduta. Controlla la connessione",
                    _ when ((int)response.StatusCode >= 500) => "Errore del server. Riprova più tardi",
                    _ => "Errore di connessione. Riprova"
                };
                return AuthResult.Failure(errorMessage);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return AuthResult.Failure("Impossibile connettersi al server. Controlla la connessione internet");
            }
            catch (TaskCanceledException)
            {
                return AuthResult.Failure("Richiesta scaduta. Controlla la connessione");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return AuthResult.Failure("Si è verificato un errore imprevisto");
            }
        }

        public async Task<AuthResult> RegisterAdminAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{Constants.ApiBaseUrl}/api/auth/register-admin", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (result != null)
                    {
                        await SecureStorage.SetAsync(Constants.TokenKey, result.Token);
                        await SecureStorage.SetAsync(Constants.UserIdKey, result.User.Id.ToString());
                        await SecureStorage.SetAsync(Constants.UserRoleKey, result.User.Role);
                        await SecureStorage.SetAsync(Constants.SchoolIdKey, result.User.SchoolId.ToString());
                        await SecureStorage.SetAsync(Constants.UserNameKey, result.User.FullName);
                        await SecureStorage.SetAsync(Constants.UserEmailKey, result.User.Email);

                        var notificationService = _serviceProvider.GetService<INotificationService>();
                        if (notificationService != null)
                        {
                            _ = Task.Run(async () => await notificationService.RegisterTokenAsync());
                        }

                        return AuthResult.Success();
                    }
                }
                var errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "401 Unauthorized",
                    HttpStatusCode.BadRequest => "Dati non validi. Controlla i dati inseriti e riprova.",
                    HttpStatusCode.InternalServerError => "Errore del server. Riprova tra poco",
                    HttpStatusCode.BadGateway => "Errore di comunicazione con il server. Riprova più tardi",
                    HttpStatusCode.ServiceUnavailable => "Servizio temporaneamente non disponibile",
                    HttpStatusCode.RequestTimeout => "Richiesta scaduta. Controlla la connessione",
                    _ when ((int)response.StatusCode >= 500) => "Errore del server. Riprova più tardi",
                    _ => "Errore di connessione. Riprova"
                };
                return AuthResult.Failure(errorMessage);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return AuthResult.Failure("Impossibile connettersi al server. Controlla la connessione internet");
            }
            catch (TaskCanceledException)
            {
                return AuthResult.Failure("Richiesta scaduta. Controlla la connessione");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return AuthResult.Failure("Si è verificato un errore imprevisto");
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync(Constants.TokenKey);
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(Constants.TokenKey);
        }

        public async Task<string?> GetUserRoleAsync()
        {
            return await SecureStorage.GetAsync(Constants.UserRoleKey);
        }

        public async Task<string?> GetUserNameAsync()
        {
            return await SecureStorage.GetAsync(Constants.UserNameKey);
        }

        public async Task<bool> ValidateTokenAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return false;

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{Constants.ApiBaseUrl}/api/teachers");

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            var notificationService = _serviceProvider.GetService<INotificationService>();
            if (notificationService != null)
            {
                await notificationService.UnregisterTokenAsync();
            }

            SecureStorage.Remove(Constants.TokenKey);
            SecureStorage.Remove(Constants.UserIdKey);
            SecureStorage.Remove(Constants.UserRoleKey);
            SecureStorage.Remove(Constants.SchoolIdKey);
            SecureStorage.Remove(Constants.UserNameKey);
            SecureStorage.Remove(Constants.UserEmailKey);

            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}