using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TemplateBase.Api.ViewModels.Home;
using TemplateBase.Core;

namespace TemplateBase.Api.Controllers.Home
{
    public class HomeController : BaseController
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
        public HomeController(IOptionsMonitor<ProjectEnvironment> env,
            IConfiguration configuration) : base(env)
        {
            _configuration = configuration;
        }
        #endregion Ctor

        #region Routes
        /// <summary>
        /// HomeController
        /// </summary>
        /// <returns></returns>
        [Route("/")]
        [AllowAnonymous]
        public HomeResult Home()
        {
            //Logger.Log(ControllerContext, new User { FirstName = "Anonyme" }, "🔁 - Versioning", new { });
            return new HomeResult
            {
                AppName = _configuration["Project:Name"],
                Version = _configuration["Project:Version"],
                Environment = _configuration["ASPNETCORE_ENVIRONMENT"],
                IssuerIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
        }
        #endregion Routes
    }
}
