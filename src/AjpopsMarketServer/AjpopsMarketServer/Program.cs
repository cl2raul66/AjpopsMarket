using AjpopsMarketServer.Hubs;
using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repositories;
using AjpopsMarketServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LoginRequest = AjpopsMarketServer.Models.LoginRequest;

var builder = WebApplication.CreateBuilder(args);

// Configuración de JWT
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);

var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings?.Secret ?? "clave_predeterminada_segura_temporal");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        // Configuración para permitir SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar servicios
builder.Services.AddSingleton<IUserRepository, UserInLiteDbService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

// Configurar CORS para permitir conexiones desde aplicaciones cliente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Servicios de SignalR con configuración adicional si es necesario
builder.Services.AddSignalR(options =>
{
    // Aumentar el tiempo de espera para mantener la conexión
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.KeepAliveInterval = TimeSpan.FromMinutes(1);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Configurar puertos
app.Urls.Add("http://*:5011");
app.Logger.LogInformation("La aplicación AjpopsMarketServer ha iniciado en el puerto 5011");

// Configuración de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Rutas de la API para consultas (sólo métodos GET)
app.MapGet("/api/users", async (IUserRepository repository) =>
{
    return await repository.GetAllAsync();
})
.RequireAuthorization()
.WithOpenApi()
.WithName("GetAllUsers")
.WithDisplayName("Obtener todos los usuarios");

app.MapGet("/api/users/{id}", async (IUserRepository repository, string id) =>
{
    var user = await repository.GetByIdAsync(id);
    if (user is null) return Results.NotFound();
    return Results.Ok(user);
})
.RequireAuthorization()
.WithOpenApi()
.WithName("GetUserById")
.WithDisplayName("Obtener usuario por ID");

app.MapGet("/api/users/by-email/{email}", async (IUserRepository repository, string email) =>
{
    var user = await repository.GetByEmailAsync(email);
    if (user is null) return Results.NotFound();
    return Results.Ok(user);
})
.RequireAuthorization()
.WithOpenApi()
.WithName("GetUserByEmail")
.WithDisplayName("Obtener usuario por email");

app.MapGet("/api/users/by-username/{username}", async (IUserRepository repository, string username) =>
{
    var user = await repository.GetByUserNameAsync(username);
    if (user is null) return Results.NotFound();
    return Results.Ok(user);
})
.RequireAuthorization()
.WithOpenApi()
.WithName("GetUserByUsername")
.WithDisplayName("Obtener usuario por nombre de usuario");

// Ruta para autenticación (login)
app.MapPost("/api/auth/login", async (IAuthService authService, LoginRequest loginRequest) =>
{
    var result = await authService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
    if (result.Success)
    {
        return Results.Ok(new { token = result.Token, user = result.User });
    }

    return Results.BadRequest(new { message = result.ErrorMessage });
})
.AllowAnonymous()
.WithOpenApi()
.WithName("Login")
.WithDisplayName("Iniciar sesión");

// Endpoint de SignalR para operaciones de modificación
app.MapHub<UserHub>("/hubs/users");

app.Run();