using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class SimuladorController : ControllerBase
{
    private readonly IMediator _mediator;

    public SimuladorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("enviar")]
    public async Task<IActionResult> EnviarMensaje([FromBody] string mensaje)
    {
        if (string.IsNullOrEmpty(mensaje))
        {
            return BadRequest("El mensaje no puede estar vacío.");
        }

        var resultado = await _mediator.Send(new SimuladorRequest { Mensaje = mensaje });
        return Ok(resultado);
    }
}
