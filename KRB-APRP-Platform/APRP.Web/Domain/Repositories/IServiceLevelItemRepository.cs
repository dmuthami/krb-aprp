using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IServiceLevelItemRepository
    {
        Task<IEnumerable<ServiceLevelItem>> ListAsync();
    }
}
