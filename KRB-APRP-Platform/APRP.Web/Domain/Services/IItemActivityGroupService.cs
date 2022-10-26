using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IItemActivityGroupService
    {
        Task<IEnumerable<ItemActivityGroup>> ListAsync();

        Task<ItemActivityGroupResponse> AddAsync(ItemActivityGroup itemActivityGroup);
        Task<ItemActivityGroupResponse> FindByIdAsync(long ID);
        Task<ItemActivityGroupResponse> Update(ItemActivityGroup itemActivityGroup);
        Task<ItemActivityGroupResponse> Update(long ID,ItemActivityGroup itemActivityGroup);
        Task<ItemActivityGroupResponse> RemoveAsync(long ID);
    }
}
