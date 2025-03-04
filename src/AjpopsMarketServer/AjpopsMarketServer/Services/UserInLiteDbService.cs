using AjpopsMarketServer.Helpers;
using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repositories;
using LiteDB;

namespace AjpopsMarketServer.Services;

public class UserInLiteDbService : IUserRepository, IAsyncDisposable
{
    readonly LiteDatabase db;
    readonly ILiteCollection<User> collection;

    public UserInLiteDbService()
    {
        ConnectionString connectionString = new()
        {
            Filename = FileHelper.GetFileDbPath("Users")
        };

        db = new(connectionString);
        collection = db.GetCollection<User>();

        // Asegurarse de que existe un índice para búsquedas rápidas
        collection.EnsureIndex(x => x.Email, unique: true);
        collection.EnsureIndex(x => x.UserName, unique: true);
    }

    public async Task<User> CreateAsync(CreateUserInput input)
    {
        // Verificar si ya existe usuario con mismo email o username
        if (collection.Exists(x => x.Email == input.Email))
        {
            throw new Exception("Ya existe un usuario con ese email");
        }

        if (collection.Exists(x => x.UserName == input.UserName))
        {
            throw new Exception("Nombre de usuario no disponible");
        }

        var user = new User
        {
            UserName = input.UserName,
            Email = input.Email,
            PasswordHash = PasswordHelper.HashPassword(input.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        collection.Insert(user);

        return await Task.FromResult(user);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = collection.FindAll().ToList();
        return await Task.FromResult(users);
    }

    public async Task<User> GetByIdAsync(string id)
    {
        var user = collection.FindById(id);

        if (user == null)
        {
            throw new KeyNotFoundException($"Usuario con id {id} no encontrado");
        }

        return await Task.FromResult(user);
    }

    public async Task<User> UpdateAsync(UpdateUserInput input)
    {
        var user = collection.FindById(input.Id);

        if (user == null)
        {
            throw new KeyNotFoundException($"Usuario con id {input.Id} no encontrado");
        }

        // Actualizar solo los campos proporcionados
        if (input.UserName != null)
        {
            // Verificar si el nombre de usuario ya está en uso por otro usuario
            if (collection.Exists(x => x.UserName == input.UserName && x.Id != input.Id))
            {
                throw new Exception("Nombre de usuario no disponible");
            }
            user.UserName = input.UserName;
        }

        if (input.Email != null)
        {
            // Verificar si el email ya está en uso por otro usuario
            if (collection.Exists(x => x.Email == input.Email && x.Id != input.Id))
            {
                throw new Exception("Ya existe un usuario con ese email");
            }
            user.Email = input.Email;
        }

        if (input.IsActive.HasValue)
        {
            user.IsActive = input.IsActive.Value;
        }

        collection.Update(user);

        return await Task.FromResult(user);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = collection.Delete(id);
        return await Task.FromResult(result);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = collection.FindOne(x => x.Email == email);
        return await Task.FromResult(user);
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        var user = collection.FindOne(x => x.UserName == userName);
        return await Task.FromResult(user);
    }

    public async Task<User> UpdateLastLoginAsync(string id)
    {
        var user = collection.FindById(id);

        if (user == null)
        {
            throw new KeyNotFoundException($"Usuario con id {id} no encontrado");
        }

        user.LastLogin = DateTime.UtcNow;
        collection.Update(user);

        return await Task.FromResult(user);
    }

    public async Task BeginTransaction()
    {
        db.BeginTrans();
        await Task.CompletedTask;
    }

    public async Task Commit()
    {
        db.Commit();
        await Task.CompletedTask;
    }

    public async Task Rollback()
    {
        db.Rollback();
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        db.Dispose();
        await Task.CompletedTask;
    }
}
