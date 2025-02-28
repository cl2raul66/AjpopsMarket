using LiteDB;

namespace AjpopsMarketServer.Types;

[QueryType]
public class Query
{

    public User GetUser() => new(ObjectId.NewObjectId().ToString(), DateTime.Now, string.Empty, null, null);
}
