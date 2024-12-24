using Microsoft.AspNetCore.Mvc;

namespace AllupMVC.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
