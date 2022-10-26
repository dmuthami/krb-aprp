using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class SurfaceTypeUnPavedRepository : BaseRepository, ISurfaceTypeUnPavedRepository
    {
        public SurfaceTypeUnPavedRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(SurfaceTypeUnPaved surfaceTypeUnPaved)
        {
            await _context.SurfaceTypeUnPaveds.AddAsync(surfaceTypeUnPaved);
        }

        public async Task<SurfaceTypeUnPaved> FindByIdAsync(long ID)
        {
            return await _context.SurfaceTypeUnPaveds.FindAsync(ID); 
        }

        public async Task<IEnumerable<SurfaceTypeUnPaved>> ListAsync()
        {
            return await _context.SurfaceTypeUnPaveds.ToListAsync();
        }

        public void Remove(SurfaceTypeUnPaved surfaceTypeUnPaved)
        {
            _context.SurfaceTypeUnPaveds.Remove(surfaceTypeUnPaved);
        }

        public void Update(SurfaceTypeUnPaved surfaceTypeUnPaved)
        {
            _context.SurfaceTypeUnPaveds.Update(surfaceTypeUnPaved);
        }
    }
}
