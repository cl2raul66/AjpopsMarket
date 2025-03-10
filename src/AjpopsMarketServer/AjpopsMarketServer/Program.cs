using AjpopsMarketServer.Hubs;
using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repositories;
using AjpopsMarketServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios
builder.Services
    .AddSingleton<IAuthService, AuthService>()
    .AddSingleton<IUserRepository, UserInLiteDbService>()
    .AddSingleton<ISendSimpleMessage, SendSimpleMessage>();
    // Registrar nuevos repositorios para productos, catálogos, categorías y órdenes
    //.AddSingleton<IProductRepository, ProductRepository>()
    //.AddSingleton<ICatalogRepository, CatalogRepository>()
    //.AddSingleton<ICategoryRepository, CategoryRepository>()
    //.AddSingleton<IOrderRepository, OrderRepository>();

// Configuración de JWT
var secret = builder.Configuration["JWT_KEY"]; // Obtener el secreto desde secrets.json

if (string.IsNullOrEmpty(secret))
{
    throw new InvalidOperationException("JWT Secret no está configurado. Por favor, configure JwtSettings:Secret en secrets.json");
}
var key = Encoding.ASCII.GetBytes(secret);

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

// Configurar Swagger con autenticación
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AjpopsMarket API", Version = "v1" });

    // Configurar autenticación JWT para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Configurar CORS para permitir conexiones desde aplicaciones cliente específicas
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://ajpops-market.company.com") // Reemplaza con tus orígenes reales
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Necesario para SignalR
    });
});

// Servicios de SignalR con configuración adicional
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

app.UseCors("AllowedOrigins");

// Configurar puertos para HTTP y HTTPS
app.Urls.Add("http://*:5011");
app.Logger.LogInformation("La aplicación AjpopsMarketServer ha iniciado");

// Configuración de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Middleware de manejo de excepciones global
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(new
        {
            StatusCode = 500,
            Message = "Ha ocurrido un error interno en el servidor"
        }.ToString() ?? "Error interno del servidor");
    });
});

// ------------------------
// ENDPOINTS PÚBLICOS (EXCEPCIONES)
// ------------------------

// 1. Login - Endpoint público para autenticación
app.MapPost("/auth/login", async (IAuthService authService, UpdateUserInput loginRequest) =>
{
    if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
    {
        return Results.BadRequest(new { message = "Email y contraseña son obligatorios" });
    }

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

// 2. Registro - Endpoint público para crear nuevos usuarios
app.MapPost("/auth/register", async (IUserRepository repository, IAuthService authService, CreateUserInput request) =>
{
    // Validación básica
    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
    {
        return Results.BadRequest(new { message = "Email y contraseña son obligatorios" });
    }

    // Verificar si el email ya existe
    var existingUser = await repository.GetByEmailAsync(request.Email);
    if (existingUser is not null)
    {
        return Results.BadRequest(new { message = "El correo electrónico ya está registrado" });
    }

    // Crear nuevo usuario
    var user = new User
    {
        Email = request.Email,
        // Nota: en un sistema real, deberías hashear la contraseña aquí o en el servicio
        // Asumimos que tu servicio de repositorio maneja esto internamente
    };

    var result = await authService.RegisterAsync(user, request.Password);
    if (result.Success)
    {
        return Results.Created($"/users/{result.User.Id}", new { id = result.User.Id });
    }

    return Results.BadRequest(new { message = result.ErrorMessage });
})
.AllowAnonymous()
.WithOpenApi()
.WithName("Register")
.WithDisplayName("Registrar usuario");

// 3. Registro de miembros - Endpoint público para crear nuevos miembros
app.MapPost("/auth/register-member", async (IUserRepository repository, IAuthService authService, CreateMemberInput request) =>
{
    // Validación básica
    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
    {
        return Results.BadRequest(new { message = "Email y contraseña son obligatorios" });
    }

    // Verificar si el email ya existe
    var existingUser = await repository.GetByEmailAsync(request.Email);
    if (existingUser is not null)
    {
        return Results.BadRequest(new { message = "El correo electrónico ya está registrado" });
    }

    // Crear nuevo miembro
    var member = new Member
    {
        Email = request.Email,
        Level = request.Level,
        MembershipExpiresAt = DateTime.UtcNow.AddYears(1) // Por defecto, membresía de un año
    };

    var result = await authService.RegisterAsync(member, request.Password);
    if (result.Success)
    {
        return Results.Created($"/users/{result.User.Id}", new { id = result.User.Id });
    }

    return Results.BadRequest(new { message = result.ErrorMessage });
})
.AllowAnonymous()
.WithOpenApi()
.WithName("RegisterMember")
.WithDisplayName("Registrar miembro");

// ------------------------
// ENDPOINTS PRIVADOS (CONSULTAS DE USUARIO)
// ------------------------

// 1. Obtener todos los usuarios
app.MapGet("/users", async (IUserRepository repository) =>
{
    return await repository.GetAllAsync();
})
.RequireAuthorization()
.WithOpenApi()
.WithName("GetAllUsers")
.WithDisplayName("Obtener todos los usuarios");

// 2. Obtener usuario por ID
app.MapGet("/users/{id}", async (IUserRepository repository, string id) =>
{
    var user = await repository.GetByIdAsync(id);
    if (user is null) return Results.NotFound(new { message = "Usuario no encontrado" });
    return Results.Ok(user);
})
.RequireAuthorization()
.WithOpenApi()
.WithName("GetUserById")
.WithDisplayName("Obtener usuario por ID");

// 3. Obtener usuario por email
app.MapGet("/users/by-email/{email}", async (IUserRepository repository, string email) =>
{
    var user = await repository.GetByEmailAsync(email);
    if (user is null) return Results.NotFound(new { message = "Usuario no encontrado" });
    return Results.Ok(user);
})
.RequireAuthorization()
.WithOpenApi()
.WithName("GetUserByEmail")
.WithDisplayName("Obtener usuario por email");

// ------------------------
// ENDPOINTS PRIVADOS (CONSULTAS DE PRODUCTOS)
// ------------------------

//// 1. Obtener todos los productos
//app.MapGet("/products", async (IProductRepository repository) =>
//{
//    return await repository.GetAllAsync();
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetAllProducts")
//.WithDisplayName("Obtener todos los productos");

//// 2. Obtener producto por ID
//app.MapGet("/products/{id}", async (IProductRepository repository, string id) =>
//{
//    var product = await repository.GetByIdAsync(id);
//    if (product is null) return Results.NotFound(new { message = "Producto no encontrado" });
//    return Results.Ok(product);
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetProductById")
//.WithDisplayName("Obtener producto por ID");

//// 3. Obtener productos por catálogo
//app.MapGet("/products/by-catalog/{catalogId}", async (IProductRepository repository, string catalogId) =>
//{
//    var products = await repository.GetByCatalogIdAsync(catalogId);
//    return Results.Ok(products);
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetProductsByCatalog")
//.WithDisplayName("Obtener productos por catálogo");

//// 4. Obtener productos por categoría
//app.MapGet("/products/by-category/{categoryId}", async (IProductRepository repository, string categoryId) =>
//{
//    var products = await repository.GetByCategoryIdAsync(categoryId);
//    return Results.Ok(products);
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetProductsByCategory")
//.WithDisplayName("Obtener productos por categoría");

//// ------------------------
//// ENDPOINTS PRIVADOS (CONSULTAS DE CATÁLOGOS Y CATEGORÍAS)
//// ------------------------

//// 1. Obtener todos los catálogos
//app.MapGet("/catalogs", async (ICatalogRepository repository) =>
//{
//    return await repository.GetAllAsync();
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetAllCatalogs")
//.WithDisplayName("Obtener todos los catálogos");

//// 2. Obtener catálogo por ID
//app.MapGet("/catalogs/{id}", async (ICatalogRepository repository, string id) =>
//{
//    var catalog = await repository.GetByIdAsync(id);
//    if (catalog is null) return Results.NotFound(new { message = "Catálogo no encontrado" });
//    return Results.Ok(catalog);
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetCatalogById")
//.WithDisplayName("Obtener catálogo por ID");

//// 3. Obtener categorías de un catálogo
//app.MapGet("/categories/by-catalog/{catalogId}", async (ICategoryRepository repository, string catalogId) =>
//{
//    var categories = await repository.GetByCatalogIdAsync(catalogId);
//    return Results.Ok(categories);
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetCategoriesByCatalog")
//.WithDisplayName("Obtener categorías por catálogo");

//// ------------------------
//// ENDPOINTS PRIVADOS (CONSULTAS DE ÓRDENES)
//// ------------------------

//// 1. Obtener órdenes de un usuario
//app.MapGet("/orders/by-user/{userId}", async (IOrderRepository repository, string userId) =>
//{
//    var orders = await repository.GetByUserIdAsync(userId);
//    return Results.Ok(orders);
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetOrdersByUser")
//.WithDisplayName("Obtener órdenes por usuario");

//// 2. Obtener orden por ID
//app.MapGet("/orders/{id}", async (IOrderRepository repository, string id) =>
//{
//    var order = await repository.GetByIdAsync(id);
//    if (order is null) return Results.NotFound(new { message = "Orden no encontrada" });
//    return Results.Ok(order);
//})
//.RequireAuthorization()
//.WithOpenApi()
//.WithName("GetOrderById")
//.WithDisplayName("Obtener orden por ID");

// 4. Endpoint de prueba para enviar email
app.MapGet("/send-email", async (ISendSimpleMessage emailSender) =>
{
    try
    {
        var response = await emailSender.Send();

        if (response.IsSuccessful)
        {
            return Results.Ok(new { success = true, message = response.Content });
        }

        return Results.BadRequest(new { success = false, message = response.ErrorMessage ?? "Error al enviar el email" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, message = $"Error al enviar el email: {ex.Message}" });
    }
})
.RequireAuthorization()
.WithOpenApi()
.WithName("SendEmail")
.WithDisplayName("Enviar Email");

// ------------------------
// OPERACIONES DE MODIFICACIÓN (VÍA SIGNALR)
// ------------------------

// Hubs de SignalR para todas las operaciones de modificación (crear, actualizar, eliminar)
// Requiere autenticación para conectarse
app.MapHub<UserHub>("/hubs/users").RequireAuthorization();
//app.MapHub<ProductHub>("/hubs/products").RequireAuthorization();
//app.MapHub<CatalogHub>("/hubs/catalogs").RequireAuthorization();
//app.MapHub<OrderHub>("/hubs/orders").RequireAuthorization();

app.Run();