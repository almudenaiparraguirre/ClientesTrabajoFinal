using Microsoft.AspNetCore.SignalR;
using SimuladorServicio;
using System.Threading.Tasks;

public class SimuladorHub : Hub
{
    public async Task SendMessage(MonitoringData data)
    {
        // Enviar el mensaje a todos los clientes conectados
        await Clients.All.SendAsync("ReceiveMessage", data);
    }
}
