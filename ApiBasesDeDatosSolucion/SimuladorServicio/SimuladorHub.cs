using Microsoft.AspNetCore.SignalR;
using SimuladorServicio;
using System.Threading.Tasks;

public class SimuladorHub : Hub
{
    public async Task SendMessage(MonitoringData data)
    {
        await Clients.All.SendAsync("ReceiveMessage", data);
    }

    public async Task SendAccessMessage(AccessMonitoringData data)
    {
        // Enviar el mensaje a todos los clientes conectados
        await Clients.All.SendAsync("ReceiveAccessMonitoringData", data);
    }

}

