using ApiBasesDeDatosProyecto.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IClienteService _clienteService;
    private readonly IClienteRepository _clienteRepository;
    private readonly ApiBasesDeDatosProyecto.IDentity.Serivicios.IUserService _userService;
    private readonly IPaisRepository _paisRepository;
    private readonly IMapper _mapper;
    private readonly Contexto _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IClienteService clienteService,
        IClienteRepository clienteRepository,
        ApiBasesDeDatosProyecto.IDentity.Serivicios.IUserService userService,
        IPaisRepository paisRepository,
        IMapper mapper,
        Contexto context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _clienteService = clienteService;
        _userService = userService;
        _paisRepository = paisRepository;
        _mapper = mapper;
        _clienteRepository = clienteRepository;
        _context = context;
    }

    // SuperAdmin: Puede ver todos los usuarios
    [Authorize(Roles = "SuperAdmin")]
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // SuperAdmin y Admin: Verificar el rol de un usuario
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet("verificarRol")]
    public async Task<IActionResult> VerificarRol(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles == null || !roles.Any())
        {
            return NotFound("El usuario no tiene roles asignados.");
        }

        var userRoleDto = new UserRoleDto
        {
            Email = email,
            Rol = roles.FirstOrDefault()
        };

        return Ok(userRoleDto);
    }

    // SuperAdmin: Puede borrar cualquier usuario
    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("users/{email}")]
    public async Task<IActionResult> DeleteUsuario(string email)
    {
        var usuario = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (usuario == null)
        {
            return NotFound("Usuario no encontrado");
        }

        usuario.IsDeleted = true;
        await _context.SaveChangesAsync();

        return Ok();
    }

    // SuperAdmin y Admin: Cambiar el rol de un usuario
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPost("cambiarRolUsuario")]
    public async Task<IActionResult> CambiarRolUsuario([FromBody] ChangeRoleViewModel model)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var currentRoles = await _userManager.GetRolesAsync(currentUser);

        // Evitar que los Admins puedan asignarse a sí mismos o a otros el rol de SuperAdmin
        if (currentRoles.Contains("Admin") && model.NuevoRol == "SuperAdmin")
        {
            return Forbid("Los Admins no pueden asignar el rol de SuperAdmin.");
        }

        // Buscar el usuario por su correo electrónico
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound(new ErrorResponseDTO("Usuario no encontrado."));
        }

        // Verificar que el rol nuevo existe
        if (!await _roleManager.RoleExistsAsync(model.NuevoRol))
        {
            return BadRequest(new ErrorResponseDTO("El rol no existe."));
        }

        // Cambiar los roles del usuario
        var result = await CambiarRolesUsuario(user, model.NuevoRol);
        if (!result.Succeeded)
        {
            return BadRequest(new ErrorResponseDTO("Error al cambiar el rol del usuario.", result.Errors.Select(e => e.Description).ToList()));
        }

        // Si el nuevo rol es "Admin", eliminar al usuario de la tabla de Clientes
        if (model.NuevoRol == "Admin")
        {
            var eliminarClienteResult = await EliminarCliente(model.Email);
            if (!eliminarClienteResult.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Error al eliminar el cliente.", eliminarClienteResult.Errors.Select(e => e.Description).ToList()));
            }
        }

        // Si el nuevo rol es "Client", registrar al usuario en la tabla de Clientes
        if (model.NuevoRol == "Client")
        {
            var clienteResult = await RegistrarCliente(model);
            if (!clienteResult.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Error al registrar el cliente.", clienteResult.Errors.Select(e => e.Description).ToList()));
            }
        }

        return NoContent();
    }

    // SuperAdmin y Admin: Puede actualizar cualquier usuario
    // Client: Puede actualizar solo sus propios datos
    [Authorize(Roles = "SuperAdmin,Admin,Client")]
    [HttpPut("updateUser")]
    public async Task<IActionResult> UpdateUser(string email, [FromBody] EditUserModel model)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound(new ErrorResponseDTO("Usuario no encontrado."));
        }

        // Si es un cliente, debe actualizar solo sus propios datos
        var currentUser = await _userManager.GetUserAsync(User);
        var isClient = await _userManager.IsInRoleAsync(currentUser, "Client");

        if (isClient && !string.Equals(currentUser.Email, email, System.StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        user.FullName = model.Nombre + " " + model.Apellido;
        user.DateOfBirth = model.FechaNacimiento;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ErrorResponseDTO("Error al actualizar el usuario.", errors));
        }

        return NoContent();
    }

    // SuperAdmin y Admin: Registrar nuevos usuarios
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        TryValidateModel(model);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser
        {
            FullName = $"{model.Nombre} {model.Apellido}",
            UserName = model.Email,
            Email = model.Email,
            DateOfBirth = model.FechaNacimiento,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(user);
    }

    // Cliente: Ver solo sus propios datos
    [Authorize(Roles = "Client")]
    [HttpGet("users/getUser")]
    public async Task<ActionResult<ApplicationUser>> GetUserByEmail([FromQuery] string email)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser.Email != email)
        {
            return Forbid();
        }

        var userDto = await _userService.GetUserByEmailAsync(email);
        if (userDto == null)
        {
            return NotFound(new { message = "User not found" });
        }
        return Ok(userDto);
    }

    // SuperAdmin, Admin: Inicio de sesión
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var token = _tokenService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        var errorResponse = new ErrorResponseDTO("Credenciales no validas.", new List<string> { "The email or password is incorrect." });
        return BadRequest(errorResponse);
    }

    // Métodos privados para cambiar roles y gestionar clientes se mantienen iguales
    private async Task<IdentityResult> CambiarRolesUsuario(ApplicationUser user, string nuevoRol)
    {
        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Contains(nuevoRol) && currentRoles.Count == 1)
        {
            return IdentityResult.Success;
        }

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return removeResult;
        }

        var addResult = await _userManager.AddToRoleAsync(user, nuevoRol);
        if (!addResult.Succeeded)
        {
            return addResult;
        }

        return IdentityResult.Success;
    }

    private async Task<IdentityResult> EliminarCliente(string email)
    {
        var cliente = await _clienteRepository.ObtenerPorEmail(email);
        if (cliente != null)
        {
            await _clienteRepository.EliminarClienteAsync(cliente);
            return IdentityResult.Success;
        }
        return IdentityResult.Failed(new IdentityError { Description = "Cliente no encontrado." });
    }

    private async Task<IdentityResult> RegistrarCliente(ChangeRoleViewModel model)
    {
        var clienteExistente = await _clienteRepository.ObtenerPorEmail(model.Email);
        if (clienteExistente != null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "El cliente ya está registrado." });
        }

        var pais = await _paisRepository.ObtenerPorNombre(model.Pais);
        if (pais == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "País no encontrado." });
        }

        var cliente = new Cliente
        {
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            PaisId = pais.Id,
            Empleo = model.Empleo,
            Email = model.Email,
            FechaNacimiento = model.FechaNacimiento,
        };

        await _clienteService.RegisterClientAsync(cliente);
        return IdentityResult.Success;
    }
}
