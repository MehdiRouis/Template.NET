using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Template.Core;
using Template.Core.Domain.Entities.Users;
using Template.Services.Security;
using static Argon2idHashService;

namespace Template.Services.Users
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly IHashService _passwordHashService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserSession> _userSessionRepository;
        #endregion Fields


        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userRepository"></param>
        public UserService(IHashService passwordHashService,
            IRepository<User> userRepository,
            IRepository<UserSession> userSessionRepository)
        {
            _passwordHashService = passwordHashService;
            _userRepository = userRepository;
            _userSessionRepository = userSessionRepository;
        }
        #endregion Ctor

        #region Methods

        #region Users
        /// <summary>
        /// Asynchronously determines whether the specified email address is not used by any existing user.
        /// </summary>
        /// <param name="email">The email address to check for uniqueness. Cannot be null or empty.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
        /// email address is unique; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
        {
            return !await _userRepository.TableNoTracking.AnyAsync(u => u.Email == email, cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve. Cannot be null or empty.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user with the specified
        /// email address, or null if no such user exists.</returns>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _userRepository.TableNoTracking
                .Include(x => x.UserPasswords)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates a new user with the specified details and returns the created user.
        /// </summary>
        /// <remarks>The created user is marked as active and assigned a password that expires in one
        /// year. The operation completes synchronously and does not perform any external I/O.</remarks>
        /// <param name="fullName">The full name of the user to create. Cannot be null or empty.</param>
        /// <param name="phone">The phone number of the user. Cannot be null or empty.</param>
        /// <param name="email">The email address of the user. Cannot be null or empty.</param>
        /// <param name="password">The password for the new user account. Cannot be null or empty.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the newly created user.</returns>
        public Task<User> InsertAsync(string fullName, string phone, string email, string password, CancellationToken cancellationToken = default)
        {
            var user = new User
            {
                FullName = fullName,
                Phone = phone,
                Email = email,
                UserPasswords = new List<UserPassword>
                {
                    new UserPassword
                    {
                        PasswordHash = _passwordHashService.Hash(password, HashPeppers.PasswordPepper),
                        ExpiresAt = DateTime.UtcNow.AddYears(1)
                    }
                },
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _userRepository.Insert(user);
            return Task.FromResult(user);
        }
        #endregion Users

        #region UsersSessions
        /// <summary>
        /// Creates a new user session with access and refresh token expiration times.
        /// </summary>
        /// <remarks>The access token expires 15 minutes after session creation, while the refresh token
        /// expires after 30 days. The refresh token is securely generated and hashed before being stored.</remarks>
        /// <param name="user">The user for whom the session is to be created. Cannot be null.</param>
        /// <returns>A <see cref="UserSession"/> instance representing the newly created session for the specified user.</returns>
        public async Task<(UserSession, string refreshToken)> CreateSessionAsync(User user)
        {
            var accessExpires = DateTimeOffset.UtcNow.AddMinutes(15);
            var refreshExpires = DateTimeOffset.UtcNow.AddDays(30);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshHash = _passwordHashService.Hash(refreshToken, HashPeppers.RefreshPepper);

            var session = new UserSession
            {
                UserId = user.Id,
                AccessExpiresAt = accessExpires.UtcDateTime,
                RefreshExpiresAt = refreshExpires.UtcDateTime,
                RefreshTokenHash = refreshHash
            };
            _userSessionRepository.Insert(session);
            session.User = user;
            return (session, refreshToken);
        }
        #endregion UsersSessions
        #endregion Methods
    }
}
