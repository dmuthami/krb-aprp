using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ICostesRegionService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(CostesRegion costesRegion);
        Task<GenericResponse> FindByIdAsync(int ID);
        Task<GenericResponse> Update(CostesRegion costesRegion);
        Task<GenericResponse> Update(int ID, CostesRegion costesRegion);
        Task<GenericResponse> RemoveAsync(int ID);
    }
}
