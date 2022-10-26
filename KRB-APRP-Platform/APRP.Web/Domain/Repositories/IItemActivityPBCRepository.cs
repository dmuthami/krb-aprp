using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IItemActivityPBCRepository
    {
        Task<IEnumerable<ItemActivityPBC>> ListAsync();
    }
}
