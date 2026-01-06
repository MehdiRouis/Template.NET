using Template.Core.Domain.Entities.Users;
using Template.Services.Security;

namespace Template.Services.Users
{
    public interface IUserService
    {
        #region Users
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User> InsertAsync(string fullName, string phone, string email, string password, CancellationToken cancellationToken = default);
        #endregion Users
        #region UsersSessions
        Task<UserSession?> GetSessionByIdAsync(Guid id);
        Task<(UserSession, string refreshToken)> CreateSessionAsync(User user);
        #endregion UsersSessions
    }
}
