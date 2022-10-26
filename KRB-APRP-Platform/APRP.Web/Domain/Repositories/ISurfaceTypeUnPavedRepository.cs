using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ISurfaceTypeUnPavedRepository
    {
        Task<IEnumerable<SurfaceTypeUnPaved>> ListAsync();

        Task AddAsync(SurfaceTypeUnPaved surfaceTypeUnPaved);
        Task<SurfaceTypeUnPaved> FindByIdAsync(long ID);
        void Update(SurfaceTypeUnPaved surfaceTypeUnPaved);
        void Remove(SurfaceTypeUnPaved surfaceTypeUnPaved);
    }
}
