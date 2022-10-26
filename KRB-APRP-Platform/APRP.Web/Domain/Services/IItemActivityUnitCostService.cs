using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IItemActivityUnitCostService
    {
        Task<IEnumerable<ItemActivityUnitCost>> ListAsync();
        Task<ItemActivityUnitCostResponse> AddAsync(ItemActivityUnitCost itemActivityUnitCost);
        Task<ItemActivityUnitCostResponse> FindByIdAsync(long ID);
        Task<ItemActivityUnitCostResponse> FindByActivityCodeAsync(string itemCode);
        Task<ItemActivityUnitCostResponse> Update(ItemActivityUnitCost itemActivityUnitCost);
        Task<ItemActivityUnitCostResponse> Update(long ID, ItemActivityUnitCost itemActivityUnitCost);
        Task<ItemActivityUnitCostResponse> RemoveAsync(long ID);
        Task<ItemActivityUnitCostResponse> FindByCodeAsync(string Code);
    }
}
