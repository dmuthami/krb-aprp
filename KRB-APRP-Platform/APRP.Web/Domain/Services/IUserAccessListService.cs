using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IUserAccessListService
    {
        Task<UserAccessListDataResponse> ListAsync();
        Task<UserAccessListResponse> AddAsync(UserAccessList userAccessList);
        Task<UserAccessListResponse> FindByIdAsync(long ID);
        Task<UserAccessListResponse> Update(long ID, UserAccessList userAccessList);
        Task<UserAccessListResponse> RemoveAsync(long ID);
        Task<UserAccessListResponse> FindByEmailAsync(string Email);
    }
}
