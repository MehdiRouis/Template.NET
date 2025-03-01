using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TemplateBase.Api.Middlewares
{
    public class RemoteIpAddressMiddleware
    {
        private readonly RequestDelegate _next;

        public RemoteIpAddressMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            UpdateRemoteIpAddress(context);
            await _next(context);
        }

        private void UpdateRemoteIpAddress(HttpContext context)
        {
            var request = context.Request;
            string ipAddress = request.Headers["CF-Connecting-IP"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                string xForwardedFor = request.Headers["X-Forwarded-For"];
                if (!string.IsNullOrEmpty(xForwardedFor))
                {
                    string[] ipAddresses = xForwardedFor.Split(',');
                    if (ipAddresses.Length > 0)
                    {
                        ipAddress = ipAddresses[0].Trim();
                    }
                }
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ipAddress);
            }
        }
    }
}
