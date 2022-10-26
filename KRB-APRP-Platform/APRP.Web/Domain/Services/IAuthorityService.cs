using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IAuthorityService
    {
        Task<GenericResponse> ListAsync(int PageNumber, int PageSize);
        Task<IEnumerable<Authority>> ListAsync();
        Task<IEnumerable<Authority>> ListAsync(string AuthorityType);
        Task<AuthorityResponse> FindByCodeAsync(string code);
        Task<AuthorityResponse> FindByIdAsync(long ID);
        Task<AuthorityResponse> Update(long ID, Authority authority);
        Task<AuthorityResponse> RemoveAsync(long ID);
        Task<AuthorityResponse> AddAsync(Authority authority);
    }
}
