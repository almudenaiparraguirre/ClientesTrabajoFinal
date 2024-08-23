using Microsoft.AspNetCore.SignalR;
using SimuladorServicio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MonitoringService
{
    private static readonly Random Random = new Random();
    private static readonly string[] Paises = { "México", "Estados Unidos", "Canadá", "Brasil", "Argentina" };
    private static readonly string[] Clientes = { "Cliente A", "Cliente B", "Cliente C", "Cliente D", "Cliente E" };

    private static MonitoringData GenerateRandomMonitoringData(int id)
    {
        return new MonitoringData
        {
            Id = id,
            Name = $"Transacción {id + 1}",
            PaisOrigen = Paises[Random.Next(Paises.Length)],
            PaisDestino = Paises[Random.Next(Paises.Length)],
            ClienteOrigen = Clientes[Random.Next(Clientes.Length)],
            ClienteDestino = Clientes[Random.Next(Clientes.Length)],
            ValorOrigen = Math.Round(Random.NextDouble() * 10000, 2),  // Valor monetario o métrico con 2 decimales
            ValorDestino = Math.Round(Random.NextDouble() * 10000, 2),
            Timestamp = DateTime.Now.AddSeconds(-Random.Next(0, 3600)) // Hasta 1 hora atrás
        };
    }

    private readonly IHubContext<SimuladorHub> _hubContext;

    public MonitoringService(IHubContext<SimuladorHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendRandomMessages()
    {
        var numberOfTransfers = Random.Next(10, 21); // Número de transferencias entre 10 y 20
        var data = new List<MonitoringData>();

        for (int i = 0; i < numberOfTransfers; i++)
        {
            data.Add(GenerateRandomMonitoringData(i));
        }

        // Enviar los datos generados a todos los clientes conectados
        foreach (var item in data)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", item);
        }
    }
}
