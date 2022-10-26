using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services
{
    public interface ITechnologyService
    {
        Task<IEnumerable<Technology>> ListAsync();

    }
}
