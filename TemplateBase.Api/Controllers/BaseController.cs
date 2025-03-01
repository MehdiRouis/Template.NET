using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TemplateBase.Core;

namespace TemplateBase.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "Base")]
    public class BaseController : Controller
    {

        /// <summary>
        /// Current specific environment
        /// </summary>
        protected readonly ProjectEnvironment Environment;

        public BaseController(IOptionsMonitor<ProjectEnvironment> env)
        {
            Environment = env.CurrentValue;
        }

    }
}
