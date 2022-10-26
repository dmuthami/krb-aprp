using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IWorkCategoryFundingMatrixRepository
    {
        Task<IEnumerable<WorkCategoryFundingMatrix>> ListAsync();

        Task<IEnumerable<WorkCategoryFundingMatrix>> ListAsync(long FinancialYearId);

        Task AddAsync(WorkCategoryFundingMatrix workCategoryFundingMatrix);
        Task<WorkCategoryFundingMatrix> FindByIdAsync(long ID);

        void Update(WorkCategoryFundingMatrix workCategoryFundingMatrix);
        void Update(long ID, WorkCategoryFundingMatrix workCategoryFundingMatrix);

        void Remove(WorkCategoryFundingMatrix workCategoryFundingMatrix);

        Task<WorkCategoryFundingMatrix> FindByAuthorityAndFinancialIdAsync(long AuthorityId, long FinancialYearId);
    }
}
