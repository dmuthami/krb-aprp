using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IWorkCategoryRepository
    {
        Task<IEnumerable<WorkCategory>> ListAsync();
    }
}
