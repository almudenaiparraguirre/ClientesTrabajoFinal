using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class SimuladorHub : Hub
{
    public async Task EnviarMensaje(string mensaje)
    {
        await Clients.All.SendAsync("RecibirMensaje", mensaje);
    }
}
