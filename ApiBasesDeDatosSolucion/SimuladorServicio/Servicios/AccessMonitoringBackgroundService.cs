using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace SimuladorServicio.Servicios
{
    public class AccessMonitoringBackgroundService : BackgroundService
    {
        private readonly ILogger<AccessMonitoringBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _intervalInSeconds;

        public AccessMonitoringBackgroundService(
            ILogger<AccessMonitoringBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            int intervalInSeconds = 20) // Puedes cambiar este valor según tu necesidad
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _intervalInSeconds = intervalInSeconds;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var monitoringService = scope.ServiceProvider.GetRequiredService<AccessMonitoringService>();

                        // Llama al método para enviar mensajes aleatorios
                        await monitoringService.GenerateRandomAccessMonitoringData();

                        _logger.LogInformation("Mensaje enviado a los clientes conectados.");
                    }

                    // Espera X segundos antes de la siguiente ejecución
                    await Task.Delay(TimeSpan.FromSeconds(_intervalInSeconds), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el servicio de monitoreo.");
                }
            }
        }
    }
}


