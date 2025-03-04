using AjpopsMarketClient.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AjpopsMarketClient;

public interface IAPIClientService
{
    event Action<dynamic>? OnUserCreated;
    event Action<string>? OnUserDeleted;
    event Action<dynamic>? OnUserUpdated;

    Task<dynamic> CreateMemberAsync(CreateMemberInput input);
    Task<dynamic> CreateUserAsync(CreateUserInput input);
    Task<bool> DeleteUserAsync(string userId);
    void Dispose();
    Task<IEnumerable<dynamic>> GetAllUsersAsync();
    Task<dynamic> GetUserByIdAsync(string id);
    Task<(bool Success, dynamic User, string Error)> LoginAsync(string email, string password);
    Task<dynamic> UpdateMemberAsync(UpdateMemberInput input);
    Task<dynamic> UpdateUserAsync(UpdateUserInput input);
}

public class APIClientService : IDisposable, IAPIClientService
{
    private readonly string _baseUrl;
    private readonly string _hubUrl;
    private HubConnection? _connection;
    private string? _token;
    private readonly HttpClient _httpClient;
    private bool _isConnected;

    /// <summary>
    /// Evento que se dispara cuando se crea un usuario
    /// </summary>
    public event Action<dynamic>? OnUserCreated;

    /// <summary>
    /// Evento que se dispara cuando se actualiza un usuario
    /// </summary>
    public event Action<dynamic>? OnUserUpdated;

    /// <summary>
    /// Evento que se dispara cuando se elimina un usuario
    /// </summary>
    public event Action<string>? OnUserDeleted;

    /// <summary>
    /// Constructor del servicio de usuarios
    /// </summary>
    /// <param name="baseUrl">URL base del servidor</param>
    public APIClientService(string baseUrl = "http://localhost:5011")
    {
        _baseUrl = baseUrl;
        _hubUrl = $"{_baseUrl}/hubs/users";
        _httpClient = new HttpClient();
        _isConnected = false;
    }

    /// <summary>
    /// Iniciar sesión usando el endpoint REST
    /// </summary>
    /// <param name="email">Correo electrónico</param>
    /// <param name="password">Contraseña</param>
    /// <returns>Resultado de login con token y usuario</returns>
    public async Task<(bool Success, dynamic User, string Error)> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new
            {
                email,
                password
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/auth/login", loginData);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, null, $"Error: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _token = result.GetProperty("token").GetString();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            // Inicializar la conexión SignalR después de iniciar sesión
            var connectionResult = await InitSignalRConnectionAsync();
            if (!connectionResult)
            {
                return (false, null, "Error al establecer conexión con SignalR");
            }

            return (true, result.GetProperty("user"), null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Error de conexión: {ex.Message}");
        }
    }

    /// <summary>
    /// Inicializar la conexión de SignalR
    /// </summary>
    /// <returns>True si la conexión se establece correctamente</returns>
    private async Task<bool> InitSignalRConnectionAsync()
    {
        try
        {
            // Crear la conexión con el token de autenticación
            _connection = new HubConnectionBuilder()
                .WithUrl($"{_hubUrl}?access_token={_token}")
                .WithAutomaticReconnect()
                .Build();

            // Registrar métodos para recibir notificaciones
            RegisterSignalREvents();

            // Iniciar la conexión
            await _connection.StartAsync();
            _isConnected = true;
            Console.WriteLine("Conexión establecida con el hub de usuarios");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al establecer conexión con SignalR: {ex.Message}");
            _isConnected = false;
            return false;
        }
    }

    /// <summary>
    /// Registrar los métodos para recibir actualizaciones en tiempo real
    /// </summary>
    private void RegisterSignalREvents()
    {
        // Se ejecuta cuando se crea un usuario nuevo
        _connection.On<dynamic>("UserCreated", (user) =>
        {
            Console.WriteLine("Usuario creado");
            OnUserCreated?.Invoke(user);
        });

        // Se ejecuta cuando se actualiza un usuario
        _connection.On<dynamic>("UserUpdated", (user) =>
        {
            Console.WriteLine("Usuario actualizado");
            OnUserUpdated?.Invoke(user);
        });

        // Se ejecuta cuando se elimina un usuario
        _connection.On<string>("UserDeleted", (userId) =>
        {
            Console.WriteLine($"Usuario eliminado: {userId}");
            OnUserDeleted?.Invoke(userId);
        });
    }

    #region MÉTODOS REST PARA CONSULTAS

    /// <summary>
    /// Obtener todos los usuarios usando REST
    /// </summary>
    /// <returns>Lista de usuarios</returns>
    public async Task<IEnumerable<dynamic>> GetAllUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/users");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<dynamic>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Obtener un usuario por ID usando REST
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Usuario</returns>
    public async Task<dynamic> GetUserByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/users/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<dynamic>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener el usuario con ID {id}: {ex.Message}");
            throw;
        }
    }

    #endregion

    #region MÉTODOS SIGNALR PARA MODIFICACIONES

    /// <summary>
    /// Crear un nuevo usuario usando SignalR
    /// </summary>
    /// <param name="input">Datos del usuario a crear</param>
    /// <returns>Usuario creado</returns>
    public async Task<dynamic> CreateUserAsync(CreateUserInput input)
    {
        EnsureConnected();

        try
        {
            return await _connection.InvokeAsync<dynamic>("CreateUser", input);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear usuario: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Crear un nuevo miembro usando SignalR
    /// </summary>
    /// <param name="input">Datos del miembro a crear</param>
    /// <returns>Miembro creado</returns>
    public async Task<dynamic> CreateMemberAsync(CreateMemberInput input)
    {
        EnsureConnected();

        try
        {
            return await _connection.InvokeAsync<dynamic>("CreateMember", input);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear miembro: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Actualizar un usuario existente usando SignalR
    /// </summary>
    /// <param name="input">Datos del usuario a actualizar</param>
    /// <returns>Usuario actualizado</returns>
    public async Task<dynamic> UpdateUserAsync(UpdateUserInput input)
    {
        EnsureConnected();

        try
        {
            return await _connection.InvokeAsync<dynamic>("UpdateUser", input);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar usuario: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Actualizar un miembro existente usando SignalR
    /// </summary>
    /// <param name="input">Datos del miembro a actualizar</param>
    /// <returns>Miembro actualizado</returns>
    public async Task<dynamic> UpdateMemberAsync(UpdateMemberInput input)
    {
        EnsureConnected();

        try
        {
            return await _connection.InvokeAsync<dynamic>("UpdateMember", input);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar miembro: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Eliminar un usuario existente usando SignalR
    /// </summary>
    /// <param name="userId">ID del usuario a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    public async Task<bool> DeleteUserAsync(string userId)
    {
        EnsureConnected();

        try
        {
            await _connection.InvokeAsync("DeleteUser", userId);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
            throw;
        }
    }

    #endregion

    /// <summary>
    /// Verifica que la conexión esté establecida
    /// </summary>
    private void EnsureConnected()
    {
        if (!_isConnected || _connection.State != HubConnectionState.Connected)
        {
            throw new InvalidOperationException("No hay conexión establecida con el servidor. Inicie sesión primero.");
        }
    }

    /// <summary>
    /// Liberar recursos
    /// </summary>
    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.DisposeAsync().GetAwaiter().GetResult();
        }

        _httpClient.Dispose();
    }
}
