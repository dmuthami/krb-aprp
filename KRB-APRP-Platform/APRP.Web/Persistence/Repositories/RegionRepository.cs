using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RegionRepository : BaseRepository, IRegionRepository
    {
        public RegionRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<Region>> ListAsync()
        {
            return await _context.Regions.ToListAsync().ConfigureAwait(false);
        }


        public async Task<Region> GetRegionAsync(long regionId)
        {
            return await _context.Regions
                .Where(r => r.ID == regionId)
                .Include(c=>c.RegionToCountys).ThenInclude(c=>c.County).ThenInclude(c=>c.Constituencys)
                .SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Region>> ListRegionsByAuthority(long authorityID)
        {
            return await _context.Regions
                .Include(c => c.RegionToCountys).ThenInclude(c => c.County).ThenInclude(c => c.Constituencys)
                .Include(c => c.RegionToCountys).ThenInclude(c => c.County).ThenInclude(m=>m.Municipalitys)
                .Include(p=>p.KWSParks)
                .Where(r => r.AuthorityId == authorityID).ToListAsync().ConfigureAwait(false);
        }
    }
}
