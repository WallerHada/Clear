using Microsoft.AspNetCore.Mvc;

namespace ProjectWebAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)] // hide controller
    [Route("sad")]
    [Route("very")]
    [ApiController]
    public class CustomController : ControllerBase
    {
        [HttpGet("store")]
        public string Get()
        {
            return "it's sad store";
        }

        [HttpGet("sad")]
        public string Good()
        {
            return "Good, i had done";
        }
    }
}
