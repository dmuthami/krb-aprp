using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadWorkBudgetHeaderRepository
    {
        Task<IEnumerable<RoadWorkBudgetHeader>> ListAsync();
        Task AddAsync(RoadWorkBudgetHeader roadWorkBudgetHeader);
        Task<RoadWorkBudgetHeader> FindByIdAsync(long ID);
        void Update(RoadWorkBudgetHeader roadWorkBudgetHeader);
        void Remove(RoadWorkBudgetHeader roadWorkBudgetHeader);
        Task<RoadWorkBudgetHeader> FindByAuthorityIdForCurrentYear(long yearId,long authorityId);

        Task<RoadWorkBudgetHeader> FindByFinancialYearIdAndAuthorityID(long financialYearId, long authorityID);
    }
}
