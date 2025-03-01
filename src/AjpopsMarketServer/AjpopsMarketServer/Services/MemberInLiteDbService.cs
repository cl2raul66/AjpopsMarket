using AjpopsMarketServer.Repository;
using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Services;

public class MemberInLiteDbService : IMemberRepository
{
    public Task<MemberT> AuthorizeCatalogForMemberAsync(string memberId, string catalogId)
    {
        throw new NotImplementedException();
    }

    public Task<MemberT> CreateMemberAsync(CreateMemberInput input)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MemberT>> GetAllMembersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MemberT> GetMemberByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<MemberT> RevokeCatalogForMemberAsync(string memberId, string catalogId)
    {
        throw new NotImplementedException();
    }

    public Task<MemberT> UpdateMemberAsync(UpdateMemberInput input)
    {
        throw new NotImplementedException();
    }
}
