using Template.Api.Application.Common.Validation;
using Template.Api.Models.Dtos.Users;
using Template.Api.Models.Views.Requests.Authentication;
using Template.Api.Models.Views.Responses.Authentication;
using Template.Core.Domain.Errors.Authentication;
using Template.Core.Domain.Errors.Common;
using Template.Services.Security;
using Template.Services.Users;
using static Argon2idHashService;

namespace Template.Api.Application.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IHashService _hashService;

        private readonly IRequestValidator<SignupRequest> _signupValidator;
        private readonly IRequestValidator<SigninRequest> _signinValidator;
        #endregion Fields

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="tokenService"></param>
        /// <param name="hashService"></param>
        /// <param name="signupValidator"></param>
        /// <param name="signinValidator"></param>
        public AuthenticationService(IUserService userService,
            ITokenService tokenService,
            IHashService hashService,
            IRequestValidator<SignupRequest> signupValidator,
            IRequestValidator<SigninRequest> signinValidator)
        {
            _userService = userService;
            _tokenService = tokenService;
            _hashService = hashService;
            _signupValidator = signupValidator;
            _signinValidator = signinValidator;
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Registers a new user account using the specified signup request data.
        /// </summary>
        /// <remarks>If the email address provided in the request is already associated with an existing
        /// account, the response will indicate failure with the appropriate result code. Validation errors in the
        /// request will also result in a failed response with error details. The session information in the response
        /// can be used for subsequent authentication.</remarks>
        /// <param name="request">The signup request containing user information such as full name, phone number, email address, and password.
        /// Cannot be null.</param>
        /// <returns>A SignupResponse indicating the result of the registration attempt. The response includes success status,
        /// result code, a message, and user/session details if registration is successful.</returns>
        public async Task<SignupResponse> SignupAsync(SignupRequest request)
        {

            var validator = _signupValidator.Validate(request);

            if(!validator.IsValid)
            {
                var errors = string.Join(" ", validator.Errors.Select(e => e.Message));
                return new SignupResponse { Success = false, Message = errors, Code = SignupResultCode.InvalidRequest };
            }

            if (!await _userService.IsEmailUniqueAsync(request.Email))
            {
                return new SignupResponse { Success = false, Message = SignupErrors.EmailAlreadyExists.Message, Code = SignupResultCode.AlreadyExists };
            }

            var user = await _userService.InsertAsync(request.FullName, request.Phone, request.Email, request.Password);

            var accessExpires = DateTimeOffset.UtcNow.AddMinutes(15);
            var refreshExpires = DateTimeOffset.UtcNow.AddDays(30);

            var (session, refreshToken) = await _userService.CreateSessionAsync(user);

            var sessionDto = _tokenService.CreateSession(session, refreshToken);

            return new SignupResponse
            {
                Success = true,
                Code = SignupResultCode.Success,
                Message = SignupErrors.Success.Message,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone
                },
                Session = sessionDto
            };
        }


        public async Task<SigninResponse> SigninAsync(SigninRequest request)
        {
            var validator = _signinValidator.Validate(request);

            if (!validator.IsValid)
            {
                return new SigninResponse
                {
                    Success = false,
                    Code = SigninResultCode.InvalidRequest,
                    Message = SigninErrors.InvalidRequest.Message
                };
            }

            var user = await _userService.GetByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
            {
                return new SigninResponse
                {
                    Success = false,
                    Code = SigninResultCode.InvalidCredentials,
                    Message = SigninErrors.InvalidCredentials.Message
                };
            }

            var password = user.UserPasswords?
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (password == null || !_hashService.Verify(request.Password, password.PasswordHash, HashPeppers.PasswordPepper))
            {
                return new SigninResponse
                {
                    Success = false,
                    Code = SigninResultCode.InvalidCredentials,
                    Message = SigninErrors.InvalidCredentials.Message
                };
            }

            var (session, refreshToken) = await _userService.CreateSessionAsync(user);

            return new SigninResponse
            {
                Success = true,
                Code = SigninResultCode.Success,
                Message = SigninErrors.Success.Message,
                Session = _tokenService.CreateSession(session, refreshToken),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone
                }
            };
        }
        #endregion Methods
    }
}
