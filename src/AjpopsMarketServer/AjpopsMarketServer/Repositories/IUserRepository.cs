using AjpopsMarketServer.Models;

namespace AjpopsMarketServer.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(string id);
    Task<User> CreateAsync(CreateUserInput input);
    Task<User> UpdateAsync(UpdateUserInput input);
    Task<bool> DeleteAsync(string id);

    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserNameAsync(string userName);
    Task<User> UpdateLastLoginAsync(string id);

    Task BeginTransaction();
    Task Commit();
    Task Rollback();
}
