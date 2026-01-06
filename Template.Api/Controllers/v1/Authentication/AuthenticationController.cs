using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Template.Api.Application.Authentication;
using Template.Api.Models.Dtos.Users;
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

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="authenticationService"></param>
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        #endregion Ctor

        /// <summary>
        /// Registers a new user account using the provided signup information.
        /// </summary>
        /// <remarks>This endpoint is accessible without authentication. The response type indicates
        /// whether the signup was successful, failed due to invalid input, or failed because the user already
        /// exists.</remarks>
        /// <param name="request">The signup details for the new user account. Must include all required fields; cannot be null.</param>
        /// <returns>A result indicating the outcome of the signup operation. Returns an HTTP 200 response with the signup result
        /// if successful; otherwise, returns HTTP 400 for invalid requests or HTTP 409 if the account already exists.</returns>
        [AllowAnonymous, HttpPost("auth-api/signup")]
        public async Task<Results<Ok<SignupResponse>, BadRequest<SignupResponse>, Conflict<SignupResponse>>> Signup([FromBody] SignupRequest request)
        {
            var result = await _authenticationService.SignupAsync(request);

            if (!result.Success)
            {
                return result.Code switch
                {
                    SignupResultCode.InvalidRequest => TypedResults.BadRequest(result),
                    SignupResultCode.AlreadyExists => TypedResults.Conflict(result),
                    _ => TypedResults.BadRequest(result)
                };
            }

            return TypedResults.Ok(result);
        }

        /// <summary>
        /// Authenticates a user based on the provided sign-in credentials and returns the result of the login attempt.
        /// </summary>
        /// <param name="request">The sign-in request containing user credentials to be validated. Cannot be null.</param>
        /// <returns>A result indicating the outcome of the login attempt: an HTTP 200 OK with a sign-in response if
        /// authentication succeeds; an HTTP 400 Bad Request with details if the request is invalid; or an HTTP 401
        /// Unauthorized if the credentials are incorrect.</returns>
        [AllowAnonymous, HttpPost("auth-api/login")]
        public async Task<Results<Ok<SigninResponse>, BadRequest<SigninResponse>, UnauthorizedHttpResult>> Login([FromBody] SigninRequest request)
        {
            var result = await _authenticationService.SigninAsync(request);

            if (!result.Success)
            {
                return result.Code switch
                {
                    SigninResultCode.InvalidRequest =>
                        TypedResults.BadRequest(result),

                    SigninResultCode.InvalidCredentials =>
                        TypedResults.Unauthorized(),

                    _ =>
                        TypedResults.BadRequest(result)
                };
            }

            return TypedResults.Ok(result);
        }


        [HttpGet("auth-api/me")]
        public async Task<Results<Ok<MeResponse>, UnauthorizedHttpResult>> Me()
        {
            var result = await _authenticationService.MeAsync(User);

            if (!result.Success)
                return TypedResults.Unauthorized();

            return TypedResults.Ok(result);
        }

    }
}
