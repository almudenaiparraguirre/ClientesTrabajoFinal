using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

public class SimuladorRequestHandler : IRequestHandler<SimuladorRequest, string>
{
    private readonly IHubContext<SimuladorHub> _hubContext;

    public SimuladorRequestHandler(IHubContext<SimuladorHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task<string> Handle(SimuladorRequest request, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.SendAsync("RecibirMensaje", request.Mensaje);
        return "Mensaje enviado";
    }
}
