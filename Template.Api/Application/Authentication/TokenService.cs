using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Template.Api.Models.Dtos.Users;
using Template.Core.Domain.Entities.Users;

namespace Template.Api.Application.Authentication
{
    public class TokenService : ITokenService
    {
        #region Fields
        private readonly int _tokenExpiresInMinutes = 10;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _jwtKey;
        #endregion Fields

        #region Ctor
        public TokenService(IConfiguration configuration)
        {
            var project = configuration["Project:Name"]!.ToLowerInvariant();
            _issuer = project + "-iam";
            _audience = project + "-api";
            if (!int.TryParse(configuration["Jwt:ExpiresInMinutes"] ?? "10", out _tokenExpiresInMinutes))
            {
                _tokenExpiresInMinutes = 10;
            }
            var raw = configuration["Jwt:SigningKey"] ?? throw new ArgumentNullException("Jwt:SigningKey configuration is missing");

            _jwtKey = Encoding.UTF8.GetBytes(raw);

            if (_jwtKey.Length < 32)
                throw new InvalidOperationException("Jwt:SigningKey must be at least 256 bits (32 bytes).");
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Creates a new user session token and returns session details, including access and refresh tokens.
        /// </summary>
        /// <remarks>The access token is valid for 15 minutes from the time of creation. The returned
        /// tokens are intended for authentication and authorization in subsequent API requests.</remarks>
        /// <param name="session">The user session information containing user identity and session metadata. Cannot be null and must have a
        /// valid user associated.</param>
        /// <param name="refreshToken">The refresh token to associate with the session. Cannot be null or empty.</param>
        /// <returns>A <see cref="UserSessionDto"/> containing the generated access token, the provided refresh token, and the
        /// token expiration time.</returns>
        public UserSessionDto CreateSession(UserSession session, string refreshToken)
        {
            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(_tokenExpiresInMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, session.User!.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, session.User.Email),
                new Claim(JwtRegisteredClaimNames.Sid, session.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Nbf, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _audience),
                new Claim("ver", "1")
            };

            var key = new SymmetricSecurityKey(_jwtKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken(
                claims: claims,
                expires: expires.UtcDateTime,
                signingCredentials: creds
            );

            return new UserSessionDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken,
                ExpiresAt = expires.ToUnixTimeSeconds()
            };
        }

        #endregion Methods
    }
}
