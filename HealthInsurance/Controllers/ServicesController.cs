using Microsoft.AspNetCore.Mvc;

namespace HealthInsurance.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
