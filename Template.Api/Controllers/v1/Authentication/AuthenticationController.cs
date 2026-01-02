using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Application.Authentication;
using Template.Api.Models.Views.Requests.Authentication;
using Template.Api.Models.Views.Responses.Authentication;

namespace Template.Api.Controllers.v1.Authentication
{
    [ApiController]
    public class AuthenticationController : BaseV1Controller
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        #endregion Fields

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous, HttpPost("auth-api/signup")]
        public Results<Ok<SignupResponse>, UnauthorizedHttpResult> Signup([FromBody] SignupRequest request)
        {
            var response = new SignupResponse
            {
                Success = false,
                Message = "Erreur lors de la création du compte."
            };

            return TypedResults.Ok(response);
        }

    }
}
