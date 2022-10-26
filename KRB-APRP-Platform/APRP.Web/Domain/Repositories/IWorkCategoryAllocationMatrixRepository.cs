using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IWorkCategoryAllocationMatrixRepository
    {
        Task<IEnumerable<WorkCategoryAllocationMatrix>> ListAsync();

        Task<IEnumerable<WorkCategoryAllocationMatrix>> ListAsync(long FinancialYearId);

        Task AddAsync(WorkCategoryAllocationMatrix workCategoryAllocationMatrix);
        Task<WorkCategoryAllocationMatrix> FindByIdAsync(long ID);

        Task<WorkCategoryAllocationMatrix> FindByNameAsync(string Name);

        void Update(WorkCategoryAllocationMatrix workCategoryAllocationMatrix);
        void Update(long ID, WorkCategoryAllocationMatrix workCategoryAllocationMatrix);

        void Remove(WorkCategoryAllocationMatrix workCategoryAllocationMatrix);

        Task<WorkCategoryAllocationMatrix> FindByAuthorityAndFinancialIdAsync(long AuthorityId, long FinancialYearId, long WorkCategoryId);
        Task<IEnumerable<WorkCategoryAllocationMatrix>> GetAuthorityWorkCategoriesAsync(long AuthorityId, long FinancialYearId);
    }
}
