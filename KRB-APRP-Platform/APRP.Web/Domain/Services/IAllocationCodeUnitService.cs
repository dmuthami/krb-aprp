using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IAllocationCodeUnitService
    {
        Task<AllocationCodeUnitListResponse> ListAsync(string AuthorityType);
        Task<AllocationCodeUnitResponse> AddAsync(AllocationCodeUnit allocationCodeUnit);
        Task<AllocationCodeUnitResponse> FindByIdAsync(long ID);
        Task<AllocationCodeUnitResponse> FindByNameAsync(string Item);
        Task<AllocationCodeUnitResponse> Update(AllocationCodeUnit allocationCodeUnit);
        Task<AllocationCodeUnitResponse> Update(long ID, AllocationCodeUnit allocationCodeUnit);
        Task<AllocationCodeUnitResponse> RemoveAsync(long ID);
        Task<AllocationCodeUnitResponse> FindByAuthorityAsync(long AuthorityId);
    }
}
