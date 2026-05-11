using Microsoft.AspNetCore.Mvc;
using RealTimeCommunication.Services;

namespace RealTimeCommunication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult WebSocketDemo() => View();
        public IActionResult ApiDemo() => View();
        public IActionResult Comparison() => View();

        [HttpPost]
        public IActionResult ToggleSimulation()
        {
            SensorBroadcastService.IsRunning = !SensorBroadcastService.IsRunning;
            return Json(new { running = SensorBroadcastService.IsRunning });
        }
    }
}