using System.Collections.Concurrent;

[Route("api/[controller]")]
[ApiController]
public class ClienteMonitoringController : ControllerBase
{
    private readonly IAccessMonitoringDataRepository _repository;
    private static readonly ConcurrentBag<TaskCompletionSource<AccessMonitoringData>> _pendingRequests = new ConcurrentBag<TaskCompletionSource<AccessMonitoringData>>();

    public ClienteMonitoringController(IAccessMonitoringDataRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessMonitoringData>>> GetMigrationData()
    {
        var data = await _repository.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("last")]
    public async Task<IActionResult> GetLast()
    {
        var lastRecord = await _repository.GetLastAsync();
        if (lastRecord == null)
        {
            return NotFound(); // Devuelve 404 si no hay registros
        }
        return Ok(lastRecord);
    }

    [HttpGet("subscribe")]
    public async Task<IActionResult> Subscribe()
    {
        var tcs = new TaskCompletionSource<AccessMonitoringData>();
        _pendingRequests.Add(tcs);

        var timeoutCancellationTokenSource = new CancellationTokenSource();
        var timeout = Task.Delay(90000, timeoutCancellationTokenSource.Token); // Timeout de 90 segundos

        var completedTask = await Task.WhenAny(tcs.Task, timeout);

        timeoutCancellationTokenSource.Cancel();

        if (completedTask == timeout)
        {
            return StatusCode(204); // No Content
        }

        return Ok(await tcs.Task);
    }

    [HttpPost("notify")]
    public async Task<IActionResult> Notify()
    {
        var lastRecord = await _repository.GetLastAsync();
        foreach (var tcs in _pendingRequests)
        {
            tcs.TrySetResult(lastRecord);
        }
        _pendingRequests.Clear();
        return Ok();
    }
}
