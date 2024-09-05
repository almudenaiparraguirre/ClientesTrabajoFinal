using ApiBasesDeDatosProyecto.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
    private readonly IUserService _userService;
    private readonly IPaisRepository _paisRepository; // Añadido
    private readonly IMapper _mapper;
    private readonly Contexto _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IClienteService clienteService,
        IClienteRepository clienteRepository,
        IUserService userService,
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

    [HttpGet("activeUsers")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetActiveUsers()
    {
        var activeUsers = _context.Users.Where(u => u.IsDeleted == false).ToList();
        return Ok(activeUsers);
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

        // Manejo de roles adicionales si es necesario
        if (model.NuevoRol == "Admin")
        {
            var eliminarClienteResult = await EliminarCliente(model.Email);
            if (!eliminarClienteResult.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Error al eliminar el cliente.", eliminarClienteResult.Errors.Select(e => e.Description).ToList()));
            }
        }
        else if (model.NuevoRol == "Client")
        {
            var clienteResult = await RegistrarCliente(model);
            if (!clienteResult.Succeeded)
            {
                return BadRequest(new ErrorResponseDTO("Error al registrar el cliente.", clienteResult.Errors.Select(e => e.Description).ToList()));
            }
        }

        return NoContent();
    }


    [Authorize]
    [HttpPut("updateUser")]
    public async Task<IActionResult> UpdateUser(string email, [FromBody] EditUserModel model)
    {
        // Obtener el token JWT de la cabecera
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        // Decodificar el token
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Obtener roles desde el payload
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        var role = roleClaim?.Value;

        // Verificar que el usuario tenga el rol necesario
        if (role != "SuperAdmin" && role != "Admin")
        {
            return Forbid(); // Si no es ni SuperAdmin ni Admin, prohibir acceso
        }

        // Buscar el usuario a modificar por su email
        var userToUpdate = await _userManager.FindByEmailAsync(email);
        if (userToUpdate == null)
        {
            return NotFound(new ErrorResponseDTO("Usuario no encontrado."));
        }

        // Actualizar propiedades del usuario
        userToUpdate.FullName = $"{model.Nombre} {model.Apellido}";
        if (model.FechaNacimiento.HasValue)
        {
            userToUpdate.DateOfBirth = model.FechaNacimiento.Value;
        }
        else
        {
            Console.WriteLine("No se proporcionó una fecha de nacimiento válida.");
        }
        // Intentar actualizar el usuario
        var result = await _userManager.UpdateAsync(userToUpdate);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ErrorResponseDTO("Error al actualizar el usuario.", errors));
        }

        return NoContent(); // Actualización exitosa
    }

    [AllowAnonymous]
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
            DateOfBirth = model.DateOfBirth,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Asignar el rol predeterminado (por ejemplo, "Client") durante el registro
        await _userManager.AddToRoleAsync(user, "Client");

        return Ok(new { Message = "Usuario registrado y rol asignado con éxito.", User = user });
    }



    // Cliente: Ver solo sus propios datos
    //[Authorize(Roles = "Client")]
    [HttpGet("users/getUser")]
    public async Task<ActionResult<ApplicationUser>> GetUserByEmail([FromQuery] string email)
    {
        var currentUser = await _userManager.FindByEmailAsync(email);
        if (currentUser.Email == email)
        {
            return Ok(email);
        }

        var userRol = await _userManager.GetRolesAsync(currentUser);
        var dateOfBirth = currentUser.DateOfBirth;
        var nombreCompleto = currentUser.FullName;
        var nombreUsuario = currentUser.UserName;

        if (currentUser == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new
        {
            email = currentUser.Email,
            fullName = nombreCompleto,
            DateOfBirth = dateOfBirth,
            rol = userRol
        });
    }

    [HttpGet("users/getCompleteUserInfo")]
    public async Task<ActionResult> GetCompleteUserInfoByEmail(string email)
    {
        var currentUser = await _userManager.FindByEmailAsync(email);
        if (currentUser == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var userRol = await _userManager.GetRolesAsync(currentUser);
        var dateOfBirth = currentUser.DateOfBirth;
        var nombreCompleto = currentUser.FullName;
        var paisNombre = currentUser.UserName;

        return Ok(new
        {
            email = currentUser.Email,
            fullName = nombreCompleto,
            dateOfBirth = dateOfBirth,
            rol = userRol.FirstOrDefault(),  // Si solo necesitas un rol
            paisNombre = paisNombre
        });
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
