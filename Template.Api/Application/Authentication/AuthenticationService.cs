using Template.Services.Users;

namespace Template.Api.Application.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        #endregion Fields

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="tokenService"></param>
        public AuthenticationService(IUserService userService,
            ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }
        #endregion Ctor

        #region Methods
        
        #endregion Methods
    }
}
