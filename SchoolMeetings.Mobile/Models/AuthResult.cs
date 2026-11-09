using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolMeetings.Mobile.Models
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static AuthResult Success() => new AuthResult { IsSuccess = true };
        public static AuthResult Failure(string errorMessage) => new AuthResult { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
