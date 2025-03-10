using AjpopsMarketServer.Helpers;
using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repositories;
using LiteDB;

namespace AjpopsMarketServer.Services;

public class UserSessionInLiteDbService : IDisposable, IUserSessionRepository
{
    readonly LiteDatabase db;
    readonly ILiteCollection<UserSession> collection;

    public UserSessionInLiteDbService()
    {
        ConnectionString connectionString = new()
        {
            Filename = FileHelper.GetFileDbPath("UserSession")
        };

        db = new(connectionString);
        collection = db.GetCollection<UserSession>();
    }

    #region QUERIES
    public UserSession GetActiveSessionByUserId(string userId) => collection.FindOne(x => x.UserId == userId && x.IsActive == true);

    public UserSession GetByToken(string token) => collection.FindOne(x => x.Token == token && x.IsActive == true);

    public bool IsTokenActive(string token) => collection.Exists(x => x.Token == token && x.IsActive == true);
    #endregion

    #region COMMANDS
    public UserSession Create(UserSession session)
    {
        string id = collection.Insert(session).AsString;
        return collection.FindById(id);
    }

    public bool DeactivateAllUserSessions(string userId)
    {
        var sessions = collection.Find(x => x.UserId == userId && x.IsActive == true);
        foreach (var session in sessions)
        {
            session.IsActive = false;
            collection.Update(session);
        }
        return true;
    }

    public bool DeactivateSession(string token)
    {
        var session = collection.FindOne(x => x.Token == token && x.IsActive == true);
        if (session is null)
        {
            return false;
        }
        session.IsActive = false;
        return collection.Update(session);
    }
    #endregion

    public void BeginTransaction() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();
}
