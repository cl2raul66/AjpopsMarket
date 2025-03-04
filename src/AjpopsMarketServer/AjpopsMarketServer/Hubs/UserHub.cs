using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

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

    // Método para crear un nuevo usuario
    public async Task<User> CreateUser(CreateUserInput input)
    {
        try
        {
            await _userRepository.BeginTransaction();

            // Verificar si ya existe un usuario con el mismo email o username
            var existingEmail = await _userRepository.GetByEmailAsync(input.Email);
            if (existingEmail is not null)
            {
                await _userRepository.Rollback();
                throw new HubException("Ya existe un usuario con ese email");
            }

            var existingUsername = await _userRepository.GetByUserNameAsync(input.UserName);
            if (existingUsername != null)
            {
                await _userRepository.Rollback();
                throw new HubException("Ya existe un usuario con ese nombre de usuario");
            }

            // Crear el usuario
            var newUser = await _userRepository.CreateAsync(input);
            await _userRepository.Commit();

            // Notificar a todos los clientes conectados
            await Clients.All.SendAsync("UserCreated", newUser);

            return newUser;
        }
        catch (HubException)
        {
            throw; // Reenviar excepciones específicas del Hub
        }
        catch (Exception ex)
        {
            await _userRepository.Rollback();
            _logger.LogError(ex, "Error al crear usuario");
            throw new HubException($"Error al crear usuario: {ex.Message}");
        }
    }

    // Método para actualizar un usuario existente
    public async Task<User> UpdateUser(UpdateUserInput input)
    {
        try
        {
            await _userRepository.BeginTransaction();

            // Verificar si el usuario existe
            var existingUser = await _userRepository.GetByIdAsync(input.Id);
            if (existingUser is null)
            {
                await _userRepository.Rollback();
                throw new HubException($"No se encontró usuario con ID: {input.Id}");
            }

            // Si se está cambiando el email, verificar que no exista otro usuario con ese email
            if (!string.Equals(existingUser.Email, input.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingEmail = await _userRepository.GetByEmailAsync(input.Email!);
                if (existingEmail is not null)
                {
                    await _userRepository.Rollback();
                    throw new HubException("Ya existe un usuario con ese email");
                }
            }

            // Si se está cambiando el nombre de usuario, verificar que no exista otro con ese nombre
            if (!string.Equals(existingUser.UserName, input.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var existingUsername = await _userRepository.GetByUserNameAsync(input.UserName!);
                if (existingUsername is not null)
                {
                    await _userRepository.Rollback();
                    throw new HubException("Ya existe un usuario con ese nombre de usuario");
                }
            }

            // Actualizar el usuario
            var updatedUser = await _userRepository.UpdateAsync(input);
            await _userRepository.Commit();

            // Notificar a todos los clientes conectados
            await Clients.All.SendAsync("UserUpdated", updatedUser);

            return updatedUser;
        }
        catch (HubException)
        {
            throw; // Reenviar excepciones específicas del Hub
        }
        catch (Exception ex)
        {
            await _userRepository.Rollback();
            _logger.LogError(ex, "Error al actualizar usuario");
            throw new HubException($"Error al actualizar usuario: {ex.Message}");
        }
    }

    // Método para eliminar un usuario
    public async Task<bool> DeleteUser(string id)
    {
        try
        {
            await _userRepository.BeginTransaction();

            // Verificar si el usuario existe
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser is null)
            {
                await _userRepository.Rollback();
                throw new HubException($"No se encontró usuario con ID: {id}");
            }

            // Eliminar el usuario
            var result = await _userRepository.DeleteAsync(id);
            await _userRepository.Commit();

            if (result)
            {
                // Notificar a todos los clientes conectados
                await Clients.All.SendAsync("UserDeleted", id);
            }

            return result;
        }
        catch (HubException)
        {
            throw; // Reenviar excepciones específicas del Hub
        }
        catch (Exception ex)
        {
            await _userRepository.Rollback();
            _logger.LogError(ex, "Error al eliminar usuario");
            throw new HubException($"Error al eliminar usuario: {ex.Message}");
        }
    }

    // Método para registrar la conexión y desconexión de clientes (opcional)
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Cliente conectado: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"Cliente desconectado: {Context.ConnectionId}. Motivo: {exception?.Message ?? "Desconexión normal"}");
        await base.OnDisconnectedAsync(exception);
    }
}