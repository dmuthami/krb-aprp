using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ShoulderSurfaceTypePavedRepository : BaseRepository, IShoulderSurfaceTypePavedRepository
    {
        public ShoulderSurfaceTypePavedRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved)
        {
            await _context.ShoulderSurfaceTypePaveds.AddAsync(shoulderSurfaceTypePaved);
        }

        public async Task<ShoulderSurfaceTypePaved> FindByIdAsync(long ID)
        {
            return await _context.ShoulderSurfaceTypePaveds.FindAsync(ID); 
        }

        public async Task<IEnumerable<ShoulderSurfaceTypePaved>> ListAsync()
        {
            return await _context.ShoulderSurfaceTypePaveds.ToListAsync();
        }

        public void Remove(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved)
        {
            _context.ShoulderSurfaceTypePaveds.Remove(shoulderSurfaceTypePaved);
        }

        public void Update(ShoulderSurfaceTypePaved shoulderSurfaceTypePaved)
        {
            _context.ShoulderSurfaceTypePaveds.Update(shoulderSurfaceTypePaved);
        }
    }
}
