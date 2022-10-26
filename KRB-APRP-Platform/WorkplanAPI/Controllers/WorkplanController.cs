using Microsoft.AspNetCore.Mvc;

namespace APRP.Services.WorkplanAPI.Controllers
{
    public class WorkplanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
