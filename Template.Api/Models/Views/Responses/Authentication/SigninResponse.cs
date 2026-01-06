using Template.Api.Models.Dtos.Users;

namespace Template.Api.Models.Views.Responses.Authentication
{
    public class SigninResponse
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public UserDto? User { get; set; }
        public UserSessionDto? Session { get; set; }
        public SigninResultCode Code { get; set; }
    }

    public enum SigninResultCode
    {
        Success,
        InvalidRequest,
        InvalidCredentials
    }
}
