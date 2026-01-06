using System.Security.Claims;
using Template.Api.Models.Views.Requests.Authentication;
using Template.Api.Models.Views.Responses.Authentication;

namespace Template.Api.Application.Authentication
{
    public interface IAuthenticationService
    {
        Task<SignupResponse> SignupAsync(SignupRequest request);
        Task<SigninResponse> SigninAsync(SigninRequest request);
        Task<MeResponse> MeAsync(ClaimsPrincipal principal);
    }
}
