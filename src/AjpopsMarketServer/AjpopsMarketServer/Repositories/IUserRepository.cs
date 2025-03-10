using AjpopsMarketServer.Models;

namespace AjpopsMarketServer.Repositories;

public interface IUserRepository
{
    void BeginTransaction();
    void Commit();
    Task<User> CreateAsync(User entity);
    Task<bool> DeleteAsync(string id);
    void Dispose();
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByEmailAsync(string email);
    Task<User> GetByIdAsync(string id);
    void Rollback();
    Task<bool> UpdateAsync(UpdateUserInput input);
}
