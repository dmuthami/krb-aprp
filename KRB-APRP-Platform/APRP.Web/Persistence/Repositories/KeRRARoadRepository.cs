using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class KeRRARoadRepository : BaseRepository , IKeRRARoadRepository
    {
        public KeRRARoadRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(KerraRoad kerraRoad)
        {
            await _context.KerraRoads.AddAsync(kerraRoad).ConfigureAwait(false);
        }

        public async Task<KerraRoad> FindByIdAsync(long ID)
        {
            return await _context.KerraRoads.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<KerraRoad> FindByRoadNumberAsync(string RoadNumber)
        {
            return await _context.KerraRoads
                .Where(s=>s.RdNum==RoadNumber)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<KerraRoad> FindBySectionIdAsync(string SectionID)
        {
            var kerraRd = await _context.KerraRoads
                .Where(s => s.Section_ID == SectionID).
                FirstOrDefaultAsync().ConfigureAwait(false);
            return kerraRd;
        }

        public async Task<IEnumerable<KerraRoad>> ListAsync()
        {
            return await _context.KerraRoads.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<KerraRoad>> ListAsync(string RoadNumber)
        {
            return await _context.KerraRoads
            .Where(s => s.RdNum == RoadNumber)
            .ToListAsync()
            .ConfigureAwait(false);
        }

        public async Task<IQueryable<KerraRoad>> ListViewAsync()
        {
            IQueryable<KerraRoad> x = null;
                await Task.Run(() =>
                {
                    x = _context.KerraRoads
                    .OrderBy(o => o.RdNum);
                }).ConfigureAwait(false);

            return x;
        }

        public void Remove(KerraRoad kerraRoad)
        {
            _context.KerraRoads.Remove(kerraRoad);
        }

        public void Update(long ID, KerraRoad kerraRoad)
        {
            _context.Entry(kerraRoad).State = EntityState.Modified;
        }
    }
}
