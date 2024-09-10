using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
string urlservicio;

// 1. Agregar servicios a la aplicación
builder.Services.AddDbContext<Contexto>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Cargar la configuración de Identity desde appsettings.json
builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection("Identity"));
var identitySettings = builder.Configuration.GetSection("Identity").Get<IdentitySettings>();

// Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = identitySettings.Password.RequireDigit;
    options.Password.RequiredLength = identitySettings.Password.RequiredLength;
    options.Password.RequireLowercase = identitySettings.Password.RequireLowercase;
    options.Password.RequireNonAlphanumeric = identitySettings.Password.RequireNonAlphanumeric;
    options.Password.RequireUppercase = identitySettings.Password.RequireUppercase;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identitySettings.Lockout.DefaultLockoutMinutes);
    options.Lockout.MaxFailedAccessAttempts = identitySettings.Lockout.MaxFailedAccessAttempts;
})
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

// Configuración de CORS para permitir solicitudes desde orígenes específicos.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials());

    options.AddPolicy("AllowAzureHost", builder => 
        builder.WithOrigins("https://delightful-ocean-0ed177403.5.azurestaticapps.net")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials());
});

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageAllPolicy", policy =>
        policy.RequireClaim("Permissions", "ManageAll"));
    options.AddPolicy("ManageAdminsPolicy", policy =>
        policy.RequireClaim("Permissions", "ManageAdmins"));
    options.AddPolicy("ManageClientsPolicy", policy =>
        policy.RequireClaim("Permissions", "ManageClients"));
});

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Filter.ByExcluding(logEvent => logEvent.Level == Serilog.Events.LogEventLevel.Debug)
    .WriteTo.Console()
    .WriteTo.File("Logs/logClientes.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Usa Serilog como el logger

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Registrar repositorios
builder.Services.AddScoped<IPaisRepository, PaisRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<ClienteService>();
builder.Services.AddScoped<IVistaClientesPaisesRepository, VistaClientesPaisesRepository>();
builder.Services.AddTransient<IMonitoringDataRepository, MonitoringDataRepository>();
builder.Services.AddScoped<IAccessMonitoringDataRepository, AccessMonitoringDataRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Configurar MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Configurar SignalR
builder.Services.AddSignalR();

// Agregar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configurar middleware
if (builder.Environment.IsDevelopment())
{
    urlservicio = "https://localhost:7040/simuladorHub";
}
else
{

    urlservicio = "https://backgroundservices-cfdjahb2bygxare0.spaincentral-01.azurewebsites.net";
}

builder.Services.AddSingleton<SignalRClientService>(provider =>
new SignalRClientService(urlservicio,
    provider.GetRequiredService<IServiceScopeFactory>()));

// -------------------------------------------------------------------------------
// Paso intermedio entre el 1 y el 2 (Construye la app)
var app = builder.Build();

// Aplica las migraciones de base de datos.
ApplyMigrations(app);

// Llamar a la inicialización de datos (SeedData)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // Inicializar datos (SeedData)
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
        throw; // Re-throw the exception after logging it
    }
}

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Habilita Swagger en desarrollo.
    app.UseSwaggerUI();  // Habilita la interfaz de usuario de Swagger.
    app.UseCors("AllowLocalhost");  // Usa la política de CORS para localhost.

}
else
{
    app.UseCors("AllowAzureHost");  // Usa la política de CORS para el host de Azure.
   
}


// Redireccionar de http a https
app.UseHttpsRedirection();

// Usar CORS
//app.UseCors();  // Asegúrate de que el uso de CORS esté aquí antes de la autenticación y autorización

// Usar autenticación y autorización
app.UseAuthentication(); // Agregar autenticación
app.UseAuthorization();  // Agregar autorización

app.MapHub<NotificationHub>("/notificationHub");

// Enrutamiento
app.MapControllers();

// Start listening to the SignalR hub
var signalRClientServices = app.Services.GetServices<SignalRClientService>();
var listeningTasks = signalRClientServices.Select(service => service.StartListeningAsync());

await Task.WhenAll(listeningTasks);

// Ejecutar la aplicación
app.Run();

static void ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            // Obtiene el contexto de base de datos y aplica las migraciones.
            var context = services.GetRequiredService<Contexto>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            // Registra cualquier error que ocurra durante la aplicación de migraciones.
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
