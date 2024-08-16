var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios a la aplicaci?n
builder.Services.AddDbContext<Contexto>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<Contexto>()
    .AddDefaultTokenProviders();

// Configurar JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Cambia esto por el origen de tu frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

//Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Filter.ByExcluding(logEvent => logEvent.Level == Serilog.Events.LogEventLevel.Debug) // Excluir eventos de nivel Debug
    .WriteTo.Console()
    .WriteTo.File("Logs/logClientes.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Usa Serilog como el logger

builder.Services.AddControllers();

// Servicio para el mapeado
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Registrar repositorios
builder.Services.AddScoped<IPaisRepository, PaisRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<ClienteService>();


// Configurar MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Configurar SignalR
builder.Services.AddSignalR();


// Agregar swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the SignalR client service with the URL of the simulator's hub
builder.Services.AddSingleton<SignalRClientService>(provider =>
    new SignalRClientService("https://localhost:7040/simuladorHub"));
builder.Services.AddSingleton<SignalRClientService>(provider =>
    new SignalRClientService("https://localhost:7050/simuladorHub"));



// Paso intermedio entre el 1 y el 2 (Construye la app)
var app = builder.Build();

app.MapHub<NotificationHub>("/notificationHub");


await CreateRoles(app);


// 2. Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireccionar de http a https
app.UseHttpsRedirection();

// Usar CORS
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication(); // Agregar autenticaci?n
app.UseAuthorization();

// Enrutamiento, determina que controlador y accion se ejecutar en funcion de la URL solicitada
app.MapControllers();

// Start listening to the SignalR hub
var signalRClientServices = app.Services.GetServices<SignalRClientService>();

var listeningTasks = signalRClientServices.Select(service => service.StartListeningAsync());

await Task.WhenAll(listeningTasks);

// Ejecutar la aplicaci?n
app.Run();

async Task CreateRoles(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Client", "Admin" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}