using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Controllers.v1
{
    [ApiController]
    [Authorize]
    [Route("functions/v1")]
    public class BaseV1Controller : Controller
    {

        public BaseV1Controller()
        {
        }

    }
}
