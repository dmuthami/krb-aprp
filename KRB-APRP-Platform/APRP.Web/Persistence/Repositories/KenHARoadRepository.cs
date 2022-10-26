using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Models;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class KenHARoadRepository : BaseRepository , IKenHARoadRepository
    {
        public KenHARoadRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(KenhaRoad kenhaRoad)
        {
            await _context.KenhaRoads.AddAsync(kenhaRoad).ConfigureAwait(false);
        }

        public async Task<KenhaRoad> FindByIdAsync(long ID)
        {
            return await _context.KenhaRoads.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<KenhaRoad> FindByRoadNumberAsync(string RoadNumber)
        {
            return await _context.KenhaRoads
                .Where(s=>s.RdNum==RoadNumber)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<KenhaRoad> FindBySectionIdAsync(string SectionID)
        {
            var kenhaRd = await _context.KenhaRoads
                .Where(s => s.Section_ID == SectionID).
                FirstOrDefaultAsync().ConfigureAwait(false);
            return kenhaRd;
        }

        public async Task<IEnumerable<KenhaRoad>> ListAsync()
        {
            return await _context.KenhaRoads.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<KenhaRoad>> ListAsync(string RoadNumber)
        {
            return await _context.KenhaRoads
             .Where(s => s.RdNum == RoadNumber)
             .ToListAsync()
             .ConfigureAwait(false);
        }

        public async Task<IQueryable<KenhaRoad>> ListViewAsync()
        {
            IQueryable<KenhaRoad> x = null;
            await Task.Run(() =>
            {
                x = _context.KenhaRoads
                .OrderBy(o => o.RdNum);
            }).ConfigureAwait(false);

            return x;
        }

        public void Remove(KenhaRoad kenhaRoad)
        {
            _context.KenhaRoads.Remove(kenhaRoad);
        }

        public void Update(long ID, KenhaRoad kenhaRoad)
        {
            _context.Entry(kenhaRoad).State = EntityState.Modified;
        }
    }
}
