using Template.Api.Models.Views.Responses.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Controllers.v1.Home
{
    public class HomeController : BaseV1Controller
    {

        #region Fields
        private readonly IConfiguration _configuration;
        #endregion Fields

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        public HomeController(IConfiguration configuration) : base()
        {
            _configuration = configuration;
        }
        #endregion Ctor

        #region Routes
        /// <summary>
        /// Home
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        [AllowAnonymous]
        public HomeResponse Home()
        {
            return new HomeResponse
            {
                AppName = _configuration["Project:Name"] ?? "Inconnu",
                Version = _configuration["Project:Version"] ?? "Inconnu",
                Environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Inconnu",
                IssuerIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Inconnu"
            };
        }
        #endregion Routes
    }
}
