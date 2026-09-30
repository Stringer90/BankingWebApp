using LocalDBWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LocalDBWebAPI.Controllers
{
    [Route("api/[controller]")]  // Ensures this controller only responds to /api/home/ routes
    [ApiController]              // This specifies that this is an API controller
    public class HomeController : ControllerBase   // Use ControllerBase for APIs
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("index")]   // Explicitly define the route as /api/home/index
        public IActionResult Index()
        {
            return Ok("This is the Home API Index action!");  // Return an Ok() result with data
        }

        [HttpGet("privacy")] // Explicit route for privacy: /api/home/privacy
        public IActionResult Privacy()
        {
            return Ok("This is the Privacy API!");  // Return a simple data response
        }

        [HttpGet("error")]   // Route for errors: /api/home/error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return Problem("An error occurred", statusCode: 500);  // Return a structured error response
        }
    }
}
