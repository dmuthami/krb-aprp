using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IWorkCategoryFundingMatrixService
    {
        Task<WorkCategoryFundingMatrixListResponse> ListAsync();

        Task<WorkCategoryFundingMatrixListResponse> ListAsync(long FinancialYearId);
        Task<WorkCategoryFundingMatrixResponse> AddAsync(WorkCategoryFundingMatrix workCategoryFundingMatrix);
        Task<WorkCategoryFundingMatrixResponse> FindByIdAsync(long ID);
        Task<WorkCategoryFundingMatrixResponse> Update(WorkCategoryFundingMatrix workCategoryFundingMatrix);
        Task<WorkCategoryFundingMatrixResponse> Update(long ID, WorkCategoryFundingMatrix workCategoryFundingMatrix);
        Task<WorkCategoryFundingMatrixResponse> RemoveAsync(long ID);
        Task<WorkCategoryFundingMatrixResponse> FindByAuthorityAndFinancialIdAsync(long AuthorityId, long FinancialYearId);
    }
}
