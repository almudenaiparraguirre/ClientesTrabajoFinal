using ApiBasesDeDatosProyecto.Context;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ApiBasesDeDatosProyecto.Servicios
{
    public class TableCleanupBackgroundService : BackgroundService
    {
        private readonly ILogger<TableCleanupBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _intervalInMinutes;

        public TableCleanupBackgroundService(
            ILogger<TableCleanupBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            int intervalInMinutes = 60) // Cada 2 horas por defecto
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _intervalInMinutes = intervalInMinutes;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<Contexto>();

                        // Comando SQL para truncar las tablas
                        var truncateCommand = @"
                            TRUNCATE TABLE [dbo].[MonitoringDatas];
                            TRUNCATE TABLE [dbo].[AccessMonitoringDatas];
                            TRUNCATE TABLE [dbo].[Clientes];
                        ";

                        // Ejecuta el comando SQL usando Entity Framework
                        await context.Database.ExecuteSqlRawAsync(truncateCommand);

                        _logger.LogInformation("Tablas truncadas correctamente a las {Time}.", DateTimeOffset.Now);
                    }

                    // Espera 2 horas (o el tiempo configurado) antes de volver a ejecutar
                    await Task.Delay(TimeSpan.FromMinutes(_intervalInMinutes), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante el truncado de tablas.");
                }
            }
        }
    }
}
