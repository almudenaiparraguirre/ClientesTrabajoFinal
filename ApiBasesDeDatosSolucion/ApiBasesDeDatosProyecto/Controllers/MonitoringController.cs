using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiBasesDeDatosProyecto.Repository;
using ApiBasesDeDatosProyecto.Models; // Asegúrate de importar el namespace donde está MonitoringData

namespace ApiBasesDeDatosProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitoringController : ControllerBase
    {
        private readonly IMonitoringDataRepository _repository;
        private static readonly ConcurrentBag<TaskCompletionSource<IEnumerable<MonitoringData>>> _pendingRequests = new ConcurrentBag<TaskCompletionSource<IEnumerable<MonitoringData>>>();

        // Inyección de dependencia del repositorio
        public MonitoringController(IMonitoringDataRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MonitoringData>>> GetMigrationData()
        {
            // Uso del método GetAllAsync del repositorio para obtener los datos
            var data = await _repository.GetAllAsync();

            // Devuelve los datos obtenidos
            return Ok(data);
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> Subscribe()
        {
            var tcs = new TaskCompletionSource<IEnumerable<MonitoringData>>();
            _pendingRequests.Add(tcs);

            // Espera hasta que se complete la tarea o se cancele
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var timeout = Task.Delay(90000, timeoutCancellationTokenSource.Token); // Timeout de 30 segundos

            var completedTask = await Task.WhenAny(tcs.Task, timeout);

            // Cancela el timeout si la tarea se completó
            timeoutCancellationTokenSource.Cancel();

            if (completedTask == timeout)
            {
                // Timeout alcanzado sin datos nuevos
                return StatusCode(204); // No Content
            }

            // Envía la respuesta cuando los datos están listos
            return Ok(await tcs.Task);
        }

        // Método para simular la notificación de datos
        [HttpPost("notify")]
        public async Task<IActionResult> Notify()
        {
            var data = await _repository.GetAllAsync();
            foreach (var tcs in _pendingRequests)
            {
                tcs.TrySetResult(data);
            }
            _pendingRequests.Clear();
            return Ok();
        }
    }
}
