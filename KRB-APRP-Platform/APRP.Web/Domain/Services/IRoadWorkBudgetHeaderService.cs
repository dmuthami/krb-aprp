using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadWorkBudgetHeaderService
    {
        Task<IEnumerable<RoadWorkBudgetHeader>> ListAsync();

        Task<RoadWorkBudgetHeaderResponse> AddAsync(RoadWorkBudgetHeader roadWorkBudgetHeader);
        Task<RoadWorkBudgetHeaderResponse> FindByIdAsync(long ID);
        Task<RoadWorkBudgetHeaderResponse> FindByAuthorityIdForCurrentYear(long yearId, long authorityID);
        Task<RoadWorkBudgetHeaderResponse> Update(RoadWorkBudgetHeader roadWorkBudgetHeader);
        Task<RoadWorkBudgetHeaderResponse> RemoveAsync(long ID);

        Task<RoadWorkBudgetHeaderResponse> FindByFinancialYearIdAndAuthorityID (long financialYearId, long authorityID);
    }
}
