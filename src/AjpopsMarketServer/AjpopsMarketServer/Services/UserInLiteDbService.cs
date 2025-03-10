using AjpopsMarketServer.Helpers;
using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repositories;
using LiteDB;

namespace AjpopsMarketServer.Services;

public class UserInLiteDbService : IDisposable, IUserRepository
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

        collection.EnsureIndex(x => x.Email, unique: true);
    }

    #region QUERIES
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = collection.FindAll().ToList();
        return await Task.FromResult(users);
    }

    public async Task<User> GetByIdAsync(string id)
    {
        var user = collection.FindById(id);

        return await Task.FromResult(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = collection.FindOne(x => x.Email == email);
        return await Task.FromResult(user);
    }
    #endregion

    #region COMMAND
    public async Task<User> CreateAsync(User entity)
    {
        var id = collection.Insert(entity).AsString;

        var newUser = collection.FindById(id);

        return await Task.FromResult(newUser);
    }

    public async Task<bool> UpdateAsync(UpdateUserInput input)
    {
        var user = collection.FindById(input.Id);

        return await Task.FromResult(collection.Update(user));
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = collection.Delete(id);
        return await Task.FromResult(result);
    }
    #endregion

    public void BeginTransaction() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();
}
