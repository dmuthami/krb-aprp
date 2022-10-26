using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IAllocationService
    {
        Task<AllocationListResponse> ListAsync();
        Task<AllocationListResponse> ListAsync(long FinancialYearId);
        Task<AllocationResponse> AddAsync(Allocation allocation);
        Task<AllocationResponse> FindByIdAsync(long ID);
        Task<AllocationResponse> FindByAllocationCodeUnitIdAsync(long AllocationCodeUnitId);
        Task<AllocationResponse> Update(Allocation allocation);
        Task<AllocationResponse> Update(long ID, Allocation allocation);
        Task<AllocationResponse> RemoveAsync(long ID);
    }
}
