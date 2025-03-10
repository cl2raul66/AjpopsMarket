using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AjpopsMarketServer.Hubs;

[Authorize]
public class UserHub : Hub
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserHub> _logger;

    public UserHub(IUserRepository userRepository, ILogger<UserHub> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    // Obtiene el ID del usuario autenticado
    private string GetUserId()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new HubException("Usuario no autenticado");
        }
        return userId;
    }

    // Verifica si el usuario actual tiene permiso para realizar operaciones
    private async Task EnsureAuthorized(string action)
    {
        var userId = GetUserId();
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new HubException("Usuario no encontrado");
        }

        // Aquí podrías implementar lógica adicional basada en roles si es necesario
        // Por ejemplo:
        // if (action == "DeleteUser" && !user.IsAdmin)
        // {
        //     throw new HubException("No tiene permisos para eliminar usuarios");
        // }
    }

    // Métodos para operaciones de modificación

    public async Task CreateUser(CreateUserInput input)
    {
        await EnsureAuthorized("CreateUser");

        try
        {
            // Verificar si el email ya existe
            var existingUser = await _userRepository.GetByEmailAsync(input.Email);
            if (existingUser is not null)
            {
                throw new HubException("El correo electrónico ya está registrado");
            }

            // Guardar el nuevo usuario
            await _userRepository.CreateAsync(input);

            // Notificar a todos los clientes conectados
            await Clients.All.SendAsync("UserCreated", input);

            _logger.LogInformation($"Usuario creado: {input.Email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            throw new HubException($"Error al crear usuario: {ex.Message}");
        }
    }

    public async Task UpdateUser(UpdateUserInput input)
    {
        await EnsureAuthorized("UpdateUser");

        try
        {
            // Verificar si el usuario existe
            var existingUser = await _userRepository.GetByIdAsync(input.Id);
            if (existingUser is null)
            {
                throw new HubException("Usuario no encontrado");
            }

            // Si se cambia el email, verificar que no exista
            if (existingUser.Email != input.Email)
            {
                var userWithSameEmail = await _userRepository.GetByEmailAsync(input.Email);
                if (userWithSameEmail is not null && userWithSameEmail.Id != input.Id)
                {
                    throw new HubException("El correo electrónico ya está en uso");
                }
            }

            // Actualizar usuario
            await _userRepository.UpdateAsync(input);

            // Notificar a todos los clientes conectados
            await Clients.All.SendAsync("UserUpdated", input);

            _logger.LogInformation($"Usuario actualizado: {input.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario");
            throw new HubException($"Error al actualizar usuario: {ex.Message}");
        }
    }

    public async Task DeleteUser(string userId)
    {
        await EnsureAuthorized("DeleteUser");

        try
        {
            // Verificar si el usuario existe
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser is null)
            {
                throw new HubException("Usuario no encontrado");
            }

            // Eliminar usuario
            await _userRepository.DeleteAsync(userId);

            // Notificar a todos los clientes conectados
            await Clients.All.SendAsync("UserDeleted", userId);

            _logger.LogInformation($"Usuario eliminado: {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario");
            throw new HubException($"Error al eliminar usuario: {ex.Message}");
        }
    }

    // Otros métodos de modificación según sea necesario...

    // Método llamado cuando un cliente se conecta
    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation($"Usuario conectado: {userId}");
            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en la conexión del cliente");
            throw;
        }
    }

    // Método llamado cuando un cliente se desconecta
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"Usuario desconectado: {userId}");
            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en la desconexión del cliente");
            throw;
        }
    }
}
