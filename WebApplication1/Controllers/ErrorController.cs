using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{code}")]
        public IActionResult Index(int code)
        {
            return View("Error");
        }

        [Route("404")]
        [Route("error/404")]
        public IActionResult NotFound()
        {
            return View("Error");
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View("Error");
        }
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return statusCode switch
            {
                404 => View("NotFound"),
                403 => View("Forbidden"),
                _ => View("Error")
            };
        }
    }

}
