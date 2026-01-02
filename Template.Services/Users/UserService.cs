using Template.Core;
using Template.Core.Domain.Entities.Users;

namespace Template.Services.Users
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly IRepository<User> _userRepository;
        #endregion Fields


        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userRepository"></param>
        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion Ctor

        #region Methods

        #endregion Methods
    }
}
