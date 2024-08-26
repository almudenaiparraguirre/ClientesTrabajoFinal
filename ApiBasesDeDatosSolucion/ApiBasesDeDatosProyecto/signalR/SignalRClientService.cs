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
        _hubConnection.On<MonitoringData>("ReceiveMessage", async (data) =>
        {
            Console.WriteLine($"Mensaje recibido del simulador:");
            Console.WriteLine($"Id: {data.Id}");
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
                Console.WriteLine("Datos guardados en la base de datos.");
            }
        });

        //await _hubConnection.StartAsync();
        Console.WriteLine("Conectado al hub de SignalR");
    }
}
