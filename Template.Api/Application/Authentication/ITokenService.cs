using Template.Api.Models.Dtos.Users;
using Template.Core.Domain.Entities.Users;

namespace Template.Api.Application.Authentication
{
    public interface ITokenService
    {
        UserSessionDto CreateSession(UserSession session, string refreshToken);
    }
}
