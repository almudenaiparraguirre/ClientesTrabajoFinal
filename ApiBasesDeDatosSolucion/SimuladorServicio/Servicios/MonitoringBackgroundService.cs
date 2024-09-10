namespace SimuladorServicio.Servicios
{
    public class MonitoringBackgroundService : BackgroundService
    {
        private readonly ILogger<MonitoringBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _intervalInSeconds;

        public MonitoringBackgroundService(
            ILogger<MonitoringBackgroundService> logger,
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
                        var monitoringService = scope.ServiceProvider.GetRequiredService<MonitoringService>();

                        // Llama al método para enviar mensajes aleatorios
                        await monitoringService.SendRandomMessages();

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
