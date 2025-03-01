using AjpopsMarketServer.Enums;
using AjpopsMarketServer.Helpers;
using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repository;
using AjpopsMarketServer.Types;
using LiteDB;

namespace AjpopsMarketServer.Services;

public class UserInLiteDbService : IUserRepository
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
    }

    public Task<UserType> CreateUserAsync(CreateUserInput input)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserType>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserType> GetUserByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<UserType> UpdateUserAsync(UpdateUserInput input)
    {
        throw new NotImplementedException();
    }
}
