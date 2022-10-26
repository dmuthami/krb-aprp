using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IShoulderSurfaceTypePavedRepository
    {
        Task<IEnumerable<ShoulderSurfaceTypePaved>> ListAsync();
        Task AddAsync(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved);
        Task<ShoulderSurfaceTypePaved> FindByIdAsync(long ID);
        void Update(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved);
        void Remove(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved);
    }
}
