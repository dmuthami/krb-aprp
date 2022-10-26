using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IWorkCategoryAllocationMatrixService
    {
        Task<WorkCategoryAllocationMatrixListResponse> ListAsync();
        Task<WorkCategoryAllocationMatrixListResponse> ListAsync(long FinancialYearId);
        Task<WorkCategoryAllocationMatrixResponse> AddAsync(WorkCategoryAllocationMatrix workCategoryAllocationMatrix);
        Task<WorkCategoryAllocationMatrixResponse> FindByIdAsync(long ID);
        Task<WorkCategoryAllocationMatrixResponse> FindByNameAsync(string Name);
        Task<WorkCategoryAllocationMatrixResponse> Update(WorkCategoryAllocationMatrix workCategoryAllocationMatrix);
        Task<WorkCategoryAllocationMatrixResponse> Update(long ID, WorkCategoryAllocationMatrix workCategoryAllocationMatrix);
        Task<WorkCategoryAllocationMatrixResponse> RemoveAsync(long ID);
        Task<WorkCategoryAllocationMatrixResponse> FindByAuthorityAndFinancialIdAsync(long AuthorityId, long FinancialYearId,long WorkCategoryId);
        Task<WorkCategoryAllocationMatrixViewModelListResponse> GetAuthorityWorkCategoriesAsync(long AuthorityId, long FinancialYearId);
    }
}
