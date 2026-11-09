namespace SchoolMeetings.Mobile.Helpers
{
    public static class Constants
    {
        // URL API:
        // - Android Emulator: https://10.0.2.2:7218
        // - iOS Simulator: https://localhost:7218
        // - Device fisico: https://TUO_IP:7218 (es. https://192.168.1.100:7218)

        // URL API per piattaforma
#if (ANDROID && DEBUG)
        public const string ApiBaseUrl = "https://10.0.2.2:7218";
#else
        public const string ApiBaseUrl = "https://grippy-unremittingly-moses.ngrok-free.dev";
#endif
        // Chiavi SecureStorage
        public const string TokenKey = "auth_token";
        public const string UserIdKey = "user_id";
        public const string UserRoleKey = "user_role";
        public const string SchoolIdKey = "school_id";
        public const string UserNameKey = "user_name";
        public const string UserEmailKey = "user_email";
    }
}