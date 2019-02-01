using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sem2_WebApp.Controllers
{
    [Authorize]
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Overview()
        {
            return View();
        }

        public IActionResult Cleaners()
        {
            return View();
        }

        public IActionResult Toilets()
        {
            return View();
        }

        [Route("AdminHome/Toilets/Settings/{id}")]
        public IActionResult Settings(int id)
        {
            return View();
        }

        public IActionResult Analytics()
        {
            return View();
        }

        // Temp
        public IActionResult Randy()
        {
            return View();
        }
        
        public IActionResult Schedules()
        {
            return View();
        }

        public IActionResult Scheduler()
        {
            return View();
        }
    }
}