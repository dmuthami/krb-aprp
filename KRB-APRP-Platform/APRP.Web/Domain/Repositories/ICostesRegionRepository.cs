using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface ICostesRegionRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(CostesRegion costesRegion);
        Task<IActionResult> FindByIdAsync(int ID);
        Task<IActionResult> Update(CostesRegion costesRegion);
        Task<IActionResult> Update(int ID,CostesRegion costesRegion);
        Task<IActionResult> Remove(CostesRegion costesRegion);
    }
}
