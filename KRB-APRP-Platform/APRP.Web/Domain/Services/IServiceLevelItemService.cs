using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services
{
    public interface IServiceLevelItemService
    {
        Task<IEnumerable<ServiceLevelItem>> ListAsync();

    }
}
