using Microsoft.AspNetCore.SignalR;
using SimuladorServicio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AccessMonitoringService
{
    private static readonly Random Random = new Random();
    private static readonly string[] Nombres = { "Juan", "Ana", "Carlos", "Laura", "Luis" };
    private static readonly string[] Apellidos = { "Gómez", "Pérez", "Rodríguez", "Martínez", "Hernández" };
    private static readonly string[] Empleos = { "Ingeniero", "Médico", "Profesor", "Abogado", "Diseñador" };
    private static readonly string[] Dominios = { "example.com", "test.com", "demo.com", "mail.com", "service.com" };
    private static readonly string[] Paises = { "México", "Estados Unidos", "Canadá", "Brasil", "Argentina" };

    private static AccessMonitoringData GenerateRandomCliente()
    {
        return new AccessMonitoringData
        {
            Nombre = Nombres[Random.Next(Nombres.Length)],
            Apellido = Apellidos[Random.Next(Apellidos.Length)],
            FechaNacimiento = DateTime.Now.AddYears(-Random.Next(18, 70)), // Edad entre 18 y 70 años
            Empleo = Empleos[Random.Next(Empleos.Length)],
            PaisId = Random.Next(1, 6), // ID de país entre 1 y 5
            Pais = Paises[Random.Next(Paises.Length)],
            Email = $"{Guid.NewGuid()}@{Dominios[Random.Next(Dominios.Length)]}",
            Usuario = $"{Nombres[Random.Next(Nombres.Length)]}{Random.Next(1000, 9999)}",
            TipoAcceso = "Login",
        };
    }

    private readonly IHubContext<SimuladorHub> _hubContext;

    public AccessMonitoringService(IHubContext<SimuladorHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task GenerateRandomAccessMonitoringData()
    {
        var data = new List<AccessMonitoringData>();

        // Generar solo 2 transacciones aleatorias
        for (int i = 0; i < 1; i++)
        {
            data.Add(GenerateRandomCliente());
        }

        // Enviar los dos datos generados a todos los clientes conectados
        foreach (var item in data)
        {
            Console.WriteLine(item.ToString());  // Corregido para imprimir el objeto item, no la lista completa
            await _hubContext.Clients.All.SendAsync("ReceiveAccessMonitoringData", item);
        }
    }
}

