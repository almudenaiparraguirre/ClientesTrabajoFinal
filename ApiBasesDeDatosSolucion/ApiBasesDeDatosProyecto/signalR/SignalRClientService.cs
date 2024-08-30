using ApiBasesDeDatosProyecto.Models;
using ApiBasesDeDatosProyecto.Repository;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public class SignalRClientService
{
    private readonly HubConnection _hubConnection;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SignalRClientService(string hubUrl, IServiceScopeFactory serviceScopeFactory)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartListeningAsync()
    {
        // Manejador para MonitoringData
        _hubConnection.On<MonitoringData>("ReceiveMessage", async (data) =>
        {
            Console.WriteLine($"Mensaje recibido del simulador (MonitoringData):");
            Console.WriteLine($"Name: {data.Name}");
            Console.WriteLine($"PaisOrigen: {data.PaisOrigen}");
            Console.WriteLine($"PaisDestino: {data.PaisDestino}");
            Console.WriteLine($"ClienteOrigen: {data.ClienteOrigen}");
            Console.WriteLine($"ClienteDestino: {data.ClienteDestino}");
            Console.WriteLine($"ValorOrigen: {data.ValorOrigen}");
            Console.WriteLine($"ValorDestino: {data.ValorDestino}");
            Console.WriteLine($"Timestamp: {data.Timestamp}");

            // Usar un scope para obtener el servicio Scoped y guardar en la base de datos
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var monitoringDataService = scope.ServiceProvider.GetRequiredService<IMonitoringDataRepository>();
                await monitoringDataService.AddAsync(data);
                Console.WriteLine("Datos de MonitoringData guardados en la base de datos.");
            }
        });

        // Manejador para AccessMonitoringData
        _hubConnection.On<AccessMonitoringData>("ReceiveAccessMonitoringData", async (data) =>
        {
            Console.WriteLine($"Mensaje recibido del simulador (AccessMonitoringData):");
            Console.WriteLine($"Nombre: {data.Nombre}");
            Console.WriteLine($"Apellido: {data.Apellido}");
            Console.WriteLine($"FechaNacimiento: {data.FechaNacimiento}");
            Console.WriteLine($"Empleo: {data.Empleo}");
            Console.WriteLine($"PaisId: {data.PaisId}");
            Console.WriteLine($"Pais: {data.Pais}");
            Console.WriteLine($"Email: {data.Email}");
            data.FechaRecibido = DateTime.Now;


            // Usar un scope para obtener el servicio Scoped y guardar en la base de datos
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Obtener AccessMonitoringDataRepository desde el scope
                var accessMonitoringDataRepository = scope.ServiceProvider.GetRequiredService<IAccessMonitoringDataRepository>();
                await accessMonitoringDataRepository.AddCliente(data);
                await accessMonitoringDataRepository.AddAsync(data);
                Console.WriteLine("Datos de AccessMonitoringData guardados en la base de datos.");
            }
        });

        // Iniciar la conexión a SignalR
        await _hubConnection.StartAsync();
        Console.WriteLine("Conectado al hub de SignalR");
    }
}
