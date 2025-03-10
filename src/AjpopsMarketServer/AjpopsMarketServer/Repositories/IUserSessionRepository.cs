using AjpopsMarketServer.Models;

namespace AjpopsMarketServer.Repositories;

public interface IUserSessionRepository
{
    UserSession Create(UserSession session);
    UserSession GetByToken(string token);
    UserSession GetActiveSessionByUserId(string userId);
    bool DeactivateSession(string token);
    bool DeactivateAllUserSessions(string userId);
    bool IsTokenActive(string token);
    void BeginTransaction();
    void Commit();
    void Rollback();
}
