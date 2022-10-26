using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class kwSRoadRepository : BaseRepository , IKwSRoadRepository
    {
        public kwSRoadRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(KwsRoad kwsRoad)
        {
            await _context.KwsRoads.AddAsync(kwsRoad).ConfigureAwait(false);
        }

        public async Task<KwsRoad> FindByIdAsync(long ID)
        {
            return await _context.KwsRoads.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<KwsRoad> FindByRoadNumberAsync(string RoadNumber)
        {
            return await _context.KwsRoads
                .Where(s=>s.RdNum==RoadNumber)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<KwsRoad> FindBySectionIdAsync(string SectionID)
        {
            var kwsRd = await _context.KwsRoads
                .Where(s => s.Section_ID == SectionID).
                FirstOrDefaultAsync().ConfigureAwait(false);
            return kwsRd;
        }

        public async Task<IEnumerable<KwsRoad>> ListAsync()
        {
            return await _context.KwsRoads.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<KwsRoad>> ListAsync(string RoadNumber)
        {
            return await _context.KwsRoads
             .Where(s => s.RdNum == RoadNumber)
             .ToListAsync()
             .ConfigureAwait(false);
        }

        public async Task<IQueryable<KwsRoad>> ListViewAsync()
        {
            IQueryable<KwsRoad> x = null;
            await Task.Run(() =>
            {
                x = _context.KwsRoads
                .OrderBy(o => o.RdNum);
            }).ConfigureAwait(false);

            return x;
        }

        public void Remove(KwsRoad kwsRoad)
        {
            _context.KwsRoads.Remove(kwsRoad);
        }

        public void Update(long ID, KwsRoad kwsRoad)
        {
            _context.Entry(kwsRoad).State = EntityState.Modified;
        }
    }
}
