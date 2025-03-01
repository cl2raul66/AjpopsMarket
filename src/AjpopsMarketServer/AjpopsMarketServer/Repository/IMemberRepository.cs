using AjpopsMarketServer.Types;

namespace AjpopsMarketServer.Repository;

public interface IMemberRepository
{
    Task<MemberT> AuthorizeCatalogForMemberAsync(string memberId, string catalogId);
    Task<IEnumerable<MemberT>> GetAllMembersAsync();
    Task<MemberT> CreateMemberAsync(CreateMemberInput input);
    Task<MemberT> GetMemberByIdAsync(string id);
    Task<MemberT> RevokeCatalogForMemberAsync(string memberId, string catalogId);
    Task<MemberT> UpdateMemberAsync(UpdateMemberInput input);
}
