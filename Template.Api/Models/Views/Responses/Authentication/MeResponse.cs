using Template.Api.Models.Dtos.Users;

namespace Template.Api.Models.Views.Responses.Authentication
{
    public class MeResponse
    {
        public bool Success { get; init; }
        public UserDto? User { get; init; }
    }
}
