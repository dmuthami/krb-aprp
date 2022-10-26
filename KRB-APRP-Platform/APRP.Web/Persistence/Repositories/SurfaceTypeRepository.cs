using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class SurfaceTypeRepository : BaseRepository, ISurfaceTypeRepository
    {
        public SurfaceTypeRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(SurfaceType surfaceType)
        {
            await _context.SurfaceTypes.AddAsync(surfaceType).ConfigureAwait(false);
        }

        public async Task<SurfaceType> FindByIdAsync(long ID)
        {
            return await _context.SurfaceTypes.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<SurfaceType> FindByNameAsync(string SurfaceType)
        {
            return await _context.SurfaceTypes
                .Where(x=>x.Name== SurfaceType)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<SurfaceType>> ListAsync()
        {
            return await _context.SurfaceTypes.ToListAsync().ConfigureAwait(false);
        }

        public void Remove(SurfaceType surfaceType)
        {
            _context.SurfaceTypes.Remove(surfaceType);
        }

        public void Update(SurfaceType surfaceType)
        {
            _context.SurfaceTypes.Update(surfaceType);
        }
    }
}
