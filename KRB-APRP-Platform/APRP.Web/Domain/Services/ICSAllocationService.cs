using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ICSAllocationService
    {
        Task<CSAllocationtListResponse> ListAsync();
        Task<CSAllocationtListResponse> ListAsync(long FinancialYearId);
        Task<CSAllocationtListResponse> ListAsync(long AuthorityId,long FinancialYearId);
        Task<GenericResponse> CSAllocationSummaryAsync(long FinancialYearId);
        Task<GenericResponse> CSAllocationSummaryByBudgetCeilingComputationAsync(long FinancialYearId);
        Task<CSAllocationtResponse> AddAsync(CSAllocation cSAllocation);
        Task<CSAllocationtResponse> FindByIdAsync(long ID);
        Task<CSAllocationtResponse> FindByCSAllocationEntryAsync(CSAllocation cSAllocation);
        Task<CSAllocationtResponse> Update(CSAllocation cSAllocation);
        Task<CSAllocationtResponse> Update(long ID, CSAllocation cSAllocation);
        Task<CSAllocationtResponse> RemoveAsync(long ID);
        Task<double> CSAllocationtItemSum(IList<CSAllocation> cSAllocationCollectionList);
        Task<CSAllocationtResponse> DetachFirstEntryAsync(CSAllocation cSAllocation);
    }
}
