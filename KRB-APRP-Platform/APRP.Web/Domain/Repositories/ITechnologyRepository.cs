using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ITechnologyRepository
    {
        Task<IEnumerable<Technology>> ListAsync();
    }
}
