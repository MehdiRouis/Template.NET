using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Template.Services.Users;

namespace Template.Api.Security.Sessions;

public static class JwtSessionHandler
{
    public static async Task OnTokenValidated(TokenValidatedContext ctx)
    {
        var sid = ctx.Principal!.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
        if (sid == null)
        {
            ctx.Fail("Missing SID");
            return;
        }

        var userService = ctx.HttpContext.RequestServices.GetRequiredService<IUserService>();
        var session = await userService.GetSessionByIdAsync(Guid.Parse(sid));

        if (session == null || session.RevokedAt != null || session.AccessExpiresAt <= DateTime.UtcNow)
        {
            ctx.Fail("Session invalid");
            return;
        }

        ctx.HttpContext.Items["Session"] = session;
    }
}