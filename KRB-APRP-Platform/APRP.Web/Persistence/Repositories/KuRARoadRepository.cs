using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Models;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class kuRARoadRepository : BaseRepository , IKuRARoadRepository
    {
        public kuRARoadRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(KuraRoad kuraRoad)
        {
            await _context.KuraRoads.AddAsync(kuraRoad).ConfigureAwait(false);
        }

        public async Task<KuraRoad> FindByIdAsync(long ID)
        {
            return await _context.KuraRoads.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<KuraRoad> FindByRoadNumberAsync(string RoadNumber)
        {
            var kuraRd = await _context.KuraRoads
                .Where(s => s.RdNum == RoadNumber).
                FirstOrDefaultAsync().ConfigureAwait(false);
            return kuraRd;
        }

        public async Task<IEnumerable<KuraRoad>> ListAsync()
        {
            return await _context.KuraRoads.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<KuraRoad>> ListAsync(string RoadNumber)
        {
            return await _context.KuraRoads
                .Where(s=>s.RdNum== RoadNumber)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IQueryable<KuraRoad>> ListViewAsync()
        {
            IQueryable<KuraRoad> x = null;
            await Task.Run(() =>
            {
                x = _context.KuraRoads
                .OrderBy(o => o.RdNum);
            }).ConfigureAwait(false);

            return x;
        }

        public void Remove(KuraRoad kuraRoad)
        {
            _context.KuraRoads.Remove(kuraRoad);
        }

        public void Update(long ID, KuraRoad kuraRoad)
        {
            _context.Entry(kuraRoad).State = EntityState.Modified;
        }
    }
}
