using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolMeetings.API.Data;
using SchoolMeetings.API.Helpers;
using SchoolMeetings.API.Services.Interfaces;
using System.Text.Json;

namespace SchoolMeetings.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly FirebaseSettings _firebaseSettings;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            ApplicationDbContext context,
            IOptions<FirebaseSettings> firebaseSettings,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _firebaseSettings = firebaseSettings.Value;
            _logger = logger;

            if (FirebaseApp.DefaultInstance == null)
            {
                var serviceAccountJson = File.ReadAllText(_firebaseSettings.ServiceAccountPath);
                var serviceAccountParams = JsonSerializer.Deserialize<ServiceAccountParameters>(serviceAccountJson)!;

                var credential = new ServiceAccountCredential(
                    new ServiceAccountCredential.Initializer(serviceAccountParams.client_email)
                    {
                        ProjectId = serviceAccountParams.project_id
                    }.FromPrivateKey(serviceAccountParams.private_key)
                ).ToGoogleCredential();

                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential,
                    ProjectId = _firebaseSettings.ProjectId
                });
            }
        }

        public async Task<bool> SendNotificationAsync(int userId, string title, string body, Dictionary<string, string>? data = null)
        {
            try
            {
                var tokens = await _context.NotificationTokens
                    .Where(t => t.UserId == userId)
                    .Select(t => t.Token)
                    .ToListAsync();

                if (!tokens.Any())
                {
                    _logger.LogWarning($"No tokens found for user {userId}");
                    return false;
                }

                var message = new MulticastMessage
                {
                    Tokens = tokens,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = data,
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = "default",
                            ChannelId = "default"
                        }
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Sound = "default",
                            Badge = 1
                        }
                    }
                };

                // send alert
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

                _logger.LogInformation($"Notification sent to user {userId}. Success: {response.SuccessCount}, Failure: {response.FailureCount}");

                if (response.FailureCount > 0)
                {
                    await RemoveInvalidTokensAsync(tokens, response);
                }

                return response.SuccessCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user {userId}");
                return false;
            }
        }

        public async Task<bool> SendNotificationToMultipleAsync(List<int> userIds, string title, string body, Dictionary<string, string>? data = null)
        {
            try
            {
                var allTokens = await _context.NotificationTokens
                    .Where(t => userIds.Contains(t.UserId))
                    .Select(t => t.Token)
                    .ToListAsync();

                if (!allTokens.Any())
                {
                    _logger.LogWarning($"No tokens found for users: {string.Join(", ", userIds)}");
                    return false;
                }

                var message = new MulticastMessage
                {
                    Tokens = allTokens,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = data,
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = "default",
                            ChannelId = "default"
                        }
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Sound = "default",
                            Badge = 1
                        }
                    }
                };

                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

                _logger.LogInformation($"Notification sent to {userIds.Count} users. Success: {response.SuccessCount}, Failure: {response.FailureCount}");

                if (response.FailureCount > 0)
                {
                    await RemoveInvalidTokensAsync(allTokens, response);
                }

                return response.SuccessCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to multiple users");
                return false;
            }
        }

        private async Task RemoveInvalidTokensAsync(List<string> tokens, BatchResponse response)
        {
            var invalidTokens = new List<string>();

            for (int i = 0; i < response.Responses.Count; i++)
            {
                var sendResponse = response.Responses[i];
                if (!sendResponse.IsSuccess)
                {
                    var errorCode = sendResponse.Exception?.MessagingErrorCode;
                    if (errorCode == MessagingErrorCode.InvalidArgument ||
                        errorCode == MessagingErrorCode.Unregistered)
                    {
                        invalidTokens.Add(tokens[i]);
                    }
                }
            }

            if (invalidTokens.Any())
            {
                var tokensToRemove = await _context.NotificationTokens
                    .Where(t => invalidTokens.Contains(t.Token))
                    .ToListAsync();

                _context.NotificationTokens.RemoveRange(tokensToRemove);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Removed {invalidTokens.Count} invalid tokens");
            }
        }
    }
}