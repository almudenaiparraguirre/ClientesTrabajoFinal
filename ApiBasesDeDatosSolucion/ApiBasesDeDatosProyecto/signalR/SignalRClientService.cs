// Services/SignalRClientService.cs
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using HubConnection = Microsoft.AspNetCore.SignalR.Client.HubConnection;

public class SignalRClientService
{
    private readonly HubConnection _hubConnection;

    public SignalRClientService(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();
    }

    public async Task StartListeningAsync()
    {
        // Configura el manejo de mensajes recibidos
        _hubConnection.On<string>("RecibirMensaje", (mensaje) =>
        {
             Console.WriteLine($"Mensaje recibido del simulador: {mensaje}");
        });

        // Inicia la conexión al hub
        //await _hubConnection.StartAsync();
        Console.WriteLine("Conectado al hub de SignalR");
    }
}
