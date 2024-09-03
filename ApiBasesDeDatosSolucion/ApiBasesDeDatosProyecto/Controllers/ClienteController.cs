using ApiBasesDeDatosProyecto.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBasesDeDatosProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IPaisRepository _paisRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClienteController> _logger;
        private readonly ClienteService _clienteService;
        private readonly Contexto _contexto;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClienteController(
            IClienteRepository clienteRepository,
            IMapper mapper,
            IPaisRepository paisRepository,
            ILogger<ClienteController> logger,
            ClienteService clienteService,
            Contexto contexto,
            UserManager<ApplicationUser> userManager)
        {
            _clienteRepository = clienteRepository;
            _paisRepository = paisRepository;
            _mapper = mapper;
            _logger = logger;
            _clienteService = clienteService;
            _contexto = contexto;
            _userManager = userManager;
        }

        // GET: api/cliente
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<List<ClienteDto>>> Get()
        {
            _logger.LogInformation($"Obteniendo todos los clientes.");
            List<Cliente> lista = await _clienteRepository.ObtenerTodosAsync();
            _logger.LogInformation($"Se obtuvieron {lista.Count} clientes.");
            return Ok(_mapper.Map<List<ClienteDto>>(lista));
        }
    

    // GET api/cliente/5
    [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,Client")]
        public async Task<ActionResult<ClienteDto>> Get(int id)
        {
            _logger.LogInformation($"Obteniendo cliente con ID {id}.");
            var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
            if (cliente == null)
            {
                _logger.LogWarning($"Cliente con ID {id} no encontrado.");
                return NotFound(new ErrorResponseDTO($"No se encontraron clientes con id {id}."));
            }
            
            // Authorize the Client role to view only their own data
            if (User.IsInRole("Client") && cliente.Email != User.Identity.Name)
            {
                return Forbid("No tienes permiso para ver los datos de otro cliente.");
            }

            return Ok(_mapper.Map<ClienteDto>(cliente));
        }

        [HttpGet("GetClientesPorNombrePais")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<List<ProAlmClientePorPaisDto>>> GetClientesPorNombrePais([FromQuery] string nombre)
        {
            _logger.LogInformation($"Obteniendo clientes para el país con nombre {nombre}.");

            var pais = await _paisRepository.ObtenerPorNombre(nombre);
            if (pais == null)
            {
                _logger.LogWarning($"País con nombre {nombre} no encontrado.");
                return NotFound(new ErrorResponseDTO($"País con nombre {nombre} no encontrado."));
            }

            var clientes = await _clienteRepository.ObtenerClientesPorPaisAsync(pais.Id);
            if (clientes == null || clientes.Count == 0)
            {
                _logger.LogWarning($"No se encontraron clientes para el país con nombre {nombre}.");
                return NotFound(new ErrorResponseDTO($"No se encontraron clientes para el país con nombre {nombre}."));
            }

            return Ok(_mapper.Map<List<ProAlmClientePorPaisDto>>(clientes));
        }

        [HttpGet("ObtenerClientesPorPais/{paisId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<List<Cliente>>> ObtenerClientesPorPais(int paisId)
        {
            var clientes = await _clienteRepository.ObtenerClientesPorPaisAsync(paisId);

            if (clientes == null || clientes.Count == 0)
            {
                return NotFound();
            }

            return Ok(clientes);
        }

        [HttpGet("GetClientesGenerados")]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult<List<Cliente>> GetClientesGenerados(int count = 10)
        {
            var clientes = _clienteService.GetClientes(count);
            return Ok(clientes);
        }

        // POST api/cliente
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> Post([FromBody] ClienteDto clienteDto)
        {
            _logger.LogInformation($"Creando un nuevo cliente.");
            if (clienteDto == null)
            {
                _logger.LogWarning($"El objeto ClienteDto recibido es nulo.");
                return BadRequest($"El objeto ClienteDto no puede ser nulo.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"El modelo ClienteDto no es válido. Errores: {ModelState}");
                return BadRequest(ModelState);
            }

        var cliente = _mapper.Map<Cliente>(clienteDto);
        _clienteRepository.AgregarAsync(cliente);

        if (await _clienteRepository.GuardarCambiosAsync())
        {
            _logger.LogInformation($"Cliente con ID {cliente.Id} creado correctamente.");
            return CreatedAtAction(nameof(Get), new { id = cliente.Id }, clienteDto);
        }

            _logger.LogError($"No se pudo agregar el cliente.");
            return BadRequest($"No se pudo agregar el cliente.");
        }

        // PUT api/cliente/5
        [HttpPut("{email}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> Put(string email, [FromBody] EditViewModel clienteDto)
        {
            _logger.LogInformation($"Actualizando cliente con email {email}.");

            if (email != clienteDto.Email)
            {
                _logger.LogWarning($"Email del cliente en la solicitud ({clienteDto.Email}) no coincide con el email de la URL ({email}).");
                return BadRequest("El email del cliente no coincide.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"El modelo ClienteDto no es válido. Errores: {ModelState}");
                return BadRequest(ModelState);
            }

            // Buscar el cliente existente en la base de datos
            var clienteExistente = await _clienteRepository.ObtenerPorEmail(email);
            if (clienteExistente == null)
            {
                _logger.LogWarning($"Cliente con email {email} no encontrado.");
                return NotFound("Cliente no encontrado.");
            }

            // Authorize Admin to edit other clients, but not their own data
            if (User.IsInRole("Client") && email != User.Identity.Name)
            {
                return Forbid("No tienes permiso para editar los datos de otro cliente.");
            }

            // Mapear las propiedades del DTO al cliente existente
            clienteExistente.Nombre = clienteDto.Nombre;
            clienteExistente.Apellido = clienteDto.Apellido;
            clienteExistente.FechaNacimiento = clienteDto.FechaNacimiento;
            clienteExistente.PaisId = clienteDto.PaisId;
            clienteExistente.Empleo = clienteDto.Empleo;

        await _clienteRepository.ActualizarAsync(clienteExistente);

        // Intentar guardar los cambios en la base de datos
        if (await _clienteRepository.GuardarCambiosAsync())
        {
            _logger.LogInformation($"Cliente con email {email} actualizado correctamente.");
            return NoContent();
        }

            return Ok(new { message = "Client edited successfully." });
        }

        // DELETE api/cliente/5
        [HttpDelete("{email}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> Delete(string email)
        {
            _logger.LogInformation($"Eliminando cliente con ID {email}.");
            var cliente = await _clienteRepository.ObtenerPorEmail(email);
            if (cliente == null)
            {
                _logger.LogWarning($"Cliente con ID {email} no encontrado para eliminar.");
                return NotFound();
            }

            // Authorize Admin to delete other clients, but not their own data
            if (User.IsInRole("Client") && email != User.Identity.Name)
            {
                return Forbid("No tienes permiso para eliminar los datos de otro cliente.");
            }

        _clienteRepository.EliminarAsync(cliente);

        if (await _clienteRepository.GuardarCambiosAsync())
        {
            _logger.LogInformation($"Cliente con ID {email} eliminado correctamente.");
            return NoContent();
        }

            _logger.LogError($"No se pudo eliminar el cliente con ID {email}.");
            return BadRequest($"No se pudo eliminar el cliente.");
        }

        // Método para obtener el país por email con validación de roles
        [Authorize(Roles = "SuperAdmin,Admin,Client")]
        [HttpGet("GetPaisPorEmail")]
        public async Task<IActionResult> GetPaisPorEmail(string email)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Client") && email != currentUser.Email)
            {
                return Forbid("No tienes permiso para ver los datos de otro cliente.");
            }

            var cliente = await _contexto.Clientes
                .Include(c => c.Pais)
                .FirstOrDefaultAsync(c => c.Email == email);


            if (cliente == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            return Ok(cliente.Pais.Nombre);
        }

        // Método para obtener un cliente por email
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("GetClientePorEmail")]
        public async Task<IActionResult> GetClientePorEmail(string email)
        {
            var cliente = await _contexto.Clientes
                .FirstOrDefaultAsync(c => c.Email == email);

            if (cliente == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            return Ok(cliente);
        }
    }
}
