using AjpopsMarketServer.Enums;
using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Repository;

public interface IUserRepository
{
    Task<IEnumerable<UserType>> GetAllUsersAsync();
    Task<UserType> GetUserByIdAsync(string id);
    Task<UserType> CreateUserAsync(CreateUserInput input);
    Task<UserType> UpdateUserAsync(UpdateUserInput input);
}
