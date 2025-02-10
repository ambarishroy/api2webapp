using Microsoft.AspNetCore.Mvc;

namespace api2webapp.Controllers
{
    public class GitHubController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
