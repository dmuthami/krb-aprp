using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IItemActivityUnitCostRepository
    {
        Task<IEnumerable<ItemActivityUnitCost>> ListAsync();
        Task AddAsync(ItemActivityUnitCost itemActivityUnitCost);
        Task<ItemActivityUnitCost> FindByIdAsync(long ID);
        Task<ItemActivityUnitCost> FindByActivityCodeAsync(string itemCode);
        void Update(ItemActivityUnitCost itemActivityUnitCost);
        void Update(long ID,ItemActivityUnitCost itemActivityUnitCost);
        void Remove(ItemActivityUnitCost itemActivityUnitCost);

        Task<ItemActivityUnitCost> FindByCodeAsync(string Code);

    }
}
