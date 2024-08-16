using MediatR;

public class SimuladorRequest : IRequest<string>
{
    public string Mensaje { get; set; }
}
