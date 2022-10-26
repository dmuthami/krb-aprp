using APRP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace APRP.Web.Controllers
{
    public class HomeAPIController : Controller
    {
        private readonly ILogger<HomeAPIController> _logger;

        public HomeAPIController(ILogger<HomeAPIController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}