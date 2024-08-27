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
            Name = $"Transacción {Random.Next(10000)}",
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
        var data = new List<MonitoringData>();

        // Generar solo 2 transacciones aleatorias
        for (int i = 0; i < 1; i++)
        {
            data.Add(GenerateRandomMonitoringData(i));
        }

        // Enviar los dos datos generados a todos los clientes conectados
        foreach (var item in data)
        {
            Console.WriteLine(item.ToString());  // Corregido para imprimir el objeto item, no la lista completa
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", item);
        }
    }
}
