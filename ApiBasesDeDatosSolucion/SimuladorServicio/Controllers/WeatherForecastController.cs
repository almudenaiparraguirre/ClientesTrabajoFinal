using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace SimuladorServicio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonitoringController : ControllerBase
    {
        private readonly ILogger<MonitoringController> _logger;
        private readonly MonitoringService _monitoringService;

        public MonitoringController(ILogger<MonitoringController> logger, MonitoringService monitoringService)
        {
            _logger = logger;
            _monitoringService = monitoringService;
        }

        // Endpoint para enviar mensajes de monitoreo aleatorios
        [HttpPost("send-random-messages")]
        public async Task<IActionResult> SendRandomMessages()
        {
            try
            {
                await _monitoringService.SendRandomMessages();
                return Ok("Mensajes aleatorios enviados.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar mensajes aleatorios.");
                return StatusCode(500, "Error al enviar mensajes.");
            }
        }
    }
}
