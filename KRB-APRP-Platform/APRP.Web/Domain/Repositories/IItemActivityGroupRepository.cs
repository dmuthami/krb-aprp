using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IItemActivityGroupRepository
    {
        Task<IEnumerable<ItemActivityGroup>> ListAsync();
        Task AddAsync(ItemActivityGroup itemActivityGroup);
        Task<ItemActivityGroup> FindByIdAsync(long ID);
        void Update(ItemActivityGroup itemActivityGroup);
        void Update(long ID,ItemActivityGroup itemActivityGroup);
        void Remove(ItemActivityGroup itemActivityGroup);

    }
}
