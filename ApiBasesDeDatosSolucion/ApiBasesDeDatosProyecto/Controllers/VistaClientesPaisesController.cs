using ApiBasesDeDatosProyecto.Entities;
using ApiBasesDeDatosProyecto.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiBasesDeDatosProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VistaClientesPaisesController : ControllerBase
    {
        private readonly IVistaClientesPaisesRepository _vistaClientesPaisesRepository;

        public VistaClientesPaisesController(IVistaClientesPaisesRepository vistaClientesPaisesRepository)
        {
            _vistaClientesPaisesRepository = vistaClientesPaisesRepository;
        }

        // GET: api/VistaClientesPaises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VistaClientesPaises>>> GetVistaClientesPaises()
        {
            var vistaClientesPaises = await _vistaClientesPaisesRepository.ObtenerTodos();
            return Ok(vistaClientesPaises);
        }

        // GET: api/VistaClientesPaises/{clienteId}
        [HttpGet("{clienteId}")]
        public async Task<ActionResult<VistaClientesPaises>> GetVistaClientesPaises(int clienteId)
        {
            var vistaClientePais = await _vistaClientesPaisesRepository.ObtenerPorClienteId(clienteId);

            if (vistaClientePais == null)
            {
                return NotFound();
            }

            return Ok(vistaClientePais);
        }
    }
}

