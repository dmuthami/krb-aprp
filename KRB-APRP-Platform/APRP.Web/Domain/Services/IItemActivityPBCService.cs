using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services
{
    public interface IItemActivityPBCService
    {
        Task<IEnumerable<ItemActivityPBC>> ListAsync();

    }
}
