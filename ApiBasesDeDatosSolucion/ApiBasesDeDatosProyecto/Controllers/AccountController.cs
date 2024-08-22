using ApiBasesDeDatosProyecto.Entities;
using Humanizer;

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
        ApiBasesDeDatosProyecto.IDentity.Serivicios.IUserService userService,
        IPaisRepository paisRepository,
        IMapper mapper,
        Contexto context) // Añadido
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _clienteService = clienteService;
        _userService = userService;
        _paisRepository = paisRepository;
        _mapper = mapper; // Añadido
        _clienteRepository = clienteRepository;
        _context = context;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
    {
        // Obtener todos los usuarios sin filtrar por correo electrónico.
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

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

        // Suponiendo que quieres devolver solo el primer rol:
        var userRoleDto = new UserRoleDto
        {
            Email = email,
            Rol = roles.FirstOrDefault()
        };

        return Ok(userRoleDto);
    }


    /*[HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (result)
        {
            return NoContent(); // Devuelve 204 No Content
        }
        return NotFound(new { message = "User not found" }); // Si el usuario no se encuentra, devuelve 404
    }*/

    [HttpGet("users/getUser")]
    public async Task<ActionResult<ApplicationUser>> GetUserByEmail([FromQuery] string email)
    {
        var userDto = await _userService.GetUserByEmailAsync(email);
        if (userDto == null)
        {
            return NotFound(new { message = "User not found" });
        }
        return Ok(userDto);
        return Ok(userDto);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        // Validar el modelo antes de procesar
        TryValidateModel(model);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
            return BadRequest(ModelState);
        }

        // Aquí model.FechaNacimiento ya es un DateTime, así que se puede usar directamente
        var user = new ApplicationUser
        {
            FullName = $"{model.Nombre} {model.Apellido}",
            UserName = model.Email,
            Email = model.Email,
            DateOfBirth = model.FechaNacimiento,
        };

        // Crear el usuario
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(user);
    }



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

        // Devolver un BadRequest con un mensaje de error
        var errorResponse = new ErrorResponseDTO("Credenciales no validas.", new List<string> { "The email or password is incorrect." });
        return BadRequest(errorResponse);
    }


    

    [HttpPost("cambiarRolUsuario")]
    public async Task<IActionResult> CambiarRolUsuario([FromBody] ChangeRoleViewModel model)
    {
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

    // Método para eliminar un cliente basado en su email
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

        //DateTime fechaNac = DateTimeOffset.FromUnixTimeMilliseconds(model.FechaNacimiento).UtcDateTime;

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

    [HttpPut("updateUser")]
    public async Task<IActionResult> UpdateUser(string email, [FromBody] EditUserModel model)
    {
        // Buscar el usuario existente por su correo electrónico
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound(new ErrorResponseDTO("Usuario no encontrado."));
        }

        // Usar AutoMapper para mapear el modelo al usuario existente
        DateTime FechaNac = DateTimeOffset.FromUnixTimeMilliseconds(model.FechaNacimiento).UtcDateTime;
        user.FullName = model.Nombre + " " + model.Apellido;
        user.DateOfBirth = FechaNac;
        //_mapper.Map(model, user);

        // Actualizar el usuario en la base de datos
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ErrorResponseDTO("Error al actualizar el usuario.", errors));
        }

        // Devolver NoContent en caso de éxito
        return NoContent();
    }

    [HttpDelete("users/{email}")]
    public async Task<IActionResult> DeleteUsuario(string email)
    {
        var usuario = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (usuario == null)
        {
            return NotFound("Usuario no encontrado");
        }

        // Marcar como eliminado lógico
        usuario.IsDeleted = true;
        await _context.SaveChangesAsync();

        return Ok();
    }
}