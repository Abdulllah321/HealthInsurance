using Microsoft.AspNetCore.Mvc;

namespace HealthInsurance.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
