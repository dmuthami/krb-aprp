using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IShoulderSurfaceTypePavedService
    {
        Task<ShoulderSurfaceTypePavedListResponse> ListAsync();
        Task<ShoulderSurfaceTypePavedResponse> AddAsync(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved);
        Task<ShoulderSurfaceTypePavedResponse> FindByIdAsync(long ID);
        Task<ShoulderSurfaceTypePavedResponse> Update(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved);
        Task<ShoulderSurfaceTypePavedResponse> RemoveAsync(long ID);

    }
}
