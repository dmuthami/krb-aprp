using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services
{
    public interface IWorkCategoryService
    {
        Task<IEnumerable<WorkCategory>> ListAsync();

    }
}
