namespace Template.Api.Models.Dtos.Users
{
    public class UserSessionDto : BaseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public long ExpiresAt { get; set; }
    }
}
