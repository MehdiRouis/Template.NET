namespace Template.Api.Models.Views.Requests.Authentication
{
    public class SigninRequest
    {
        public required string Email { get; set; }

        public required string Password { get; set; }
    }

}
