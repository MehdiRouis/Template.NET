using Template.Api.Models.Views.Requests.Authentication;
using Template.Api.Models.Views.Responses.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Controllers.v1.Authentication
{
    [ApiController]
    public class AuthenticationController : BaseV1Controller
    {

        public AuthenticationController()
        {
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
