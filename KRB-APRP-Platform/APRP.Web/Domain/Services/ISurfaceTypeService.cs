using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ISurfaceTypeService
    {
        Task<SurfaceTypeListResponse> ListAsync();
        Task<SurfaceTypeResponse> AddAsync(SurfaceType surfaceType);
        Task<SurfaceTypeResponse> FindByIdAsync(long ID);
        Task<SurfaceTypeResponse> FindByNameAsync(string SurfaceType);
        Task<SurfaceTypeResponse> Update(SurfaceType surfaceType);
        Task<SurfaceTypeResponse> RemoveAsync(long ID);
    }
}
