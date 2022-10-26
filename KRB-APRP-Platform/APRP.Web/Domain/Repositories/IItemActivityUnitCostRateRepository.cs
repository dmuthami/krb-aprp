using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IItemActivityUnitCostRateRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(ItemActivityUnitCostRate itemActivityUnitCostRate);
        Task<IActionResult> FindByIdAsync(long ID);

        Task<IActionResult> FindByFinancialYearAuthorityAndItemUnitCostAsync(long FinancialYearId,
            long AuthorityId, long ItemActivityUnitCostId);
        Task<IActionResult> Update(ItemActivityUnitCostRate itemActivityUnitCostRate);
        Task<IActionResult> Remove(ItemActivityUnitCostRate itemActivityUnitCostRate);
    }
}
