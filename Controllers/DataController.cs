using Microsoft.AspNetCore.Mvc;
using RealTimeCommunication.Services;

namespace RealTimeCommunication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        [HttpGet("status")]
        public IActionResult GetStatus() => Ok(new
        {
            message = "Сервер працює",
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            simulationRunning = SensorBroadcastService.IsRunning,
            totalReadings = SensorBroadcastService.History.Count
        });

        [HttpGet("sensors/latest")]
        public IActionResult GetLatest()
        {
            var latest = SensorBroadcastService.History.LastOrDefault();
            if (latest == null) return NotFound(new { message = "Даних ще немає" });
            return Ok(latest);
        }

        [HttpGet("sensors/history")]
        public IActionResult GetHistory([FromQuery] int count = 10)
        {
            var data = SensorBroadcastService.History.TakeLast(count);
            return Ok(data);
        }

        [HttpGet("alerts")]
        public IActionResult GetAlerts() => Ok(SensorBroadcastService.Alerts);

        [HttpPost("simulation/toggle")]
        public IActionResult Toggle()
        {
            SensorBroadcastService.IsRunning = !SensorBroadcastService.IsRunning;
            return Ok(new { running = SensorBroadcastService.IsRunning });
        }
    }
}