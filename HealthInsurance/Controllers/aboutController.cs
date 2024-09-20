using Microsoft.AspNetCore.Mvc;

namespace HealthInsurance.Controllers
{
    public class aboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
