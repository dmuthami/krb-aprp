using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ISurfaceTypeRepository
    {
        Task<IEnumerable<SurfaceType>> ListAsync();

        Task AddAsync(SurfaceType surfaceType);
        Task<SurfaceType> FindByIdAsync(long ID);
        Task<SurfaceType> FindByNameAsync(string SurfaceType);
        void Update(SurfaceType surfaceType);
        void Remove(SurfaceType surfaceType);
    }
}
