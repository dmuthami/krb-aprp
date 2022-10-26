using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ISurfaceTypeUnPavedService
    {
        Task<SurfaceTypeUnPavedListResponse> ListAsync();
        Task<SurfaceTypeUnPavedResponse> AddAsync(SurfaceTypeUnPaved surfaceTypeUnPaved);
        Task<SurfaceTypeUnPavedResponse> FindByIdAsync(long ID);
        Task<SurfaceTypeUnPavedResponse> Update(SurfaceTypeUnPaved surfaceTypeUnPaved);
        Task<SurfaceTypeUnPavedResponse> RemoveAsync(long ID);

    }
}
