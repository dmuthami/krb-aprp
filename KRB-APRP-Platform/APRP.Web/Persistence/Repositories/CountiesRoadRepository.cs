using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Models;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class CountiesRoadRepository : BaseRepository , ICountiesRoadRepository
    {
        public CountiesRoadRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(CountiesRoad countiesRoad)
        {
            await _context.CountiesRoads.AddAsync(countiesRoad).ConfigureAwait(false);
        }

        public async Task<CountiesRoad> FindByIdAsync(long ID)
        {
            return await _context.CountiesRoads.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<CountiesRoad> FindByRoadNumberAsync(string RoadNumber)
        {
            var countiesRd = await _context.CountiesRoads
                .Where(s => s.RdNum == RoadNumber).
                FirstOrDefaultAsync().ConfigureAwait(false);
            return countiesRd;
        }

        public async Task<IEnumerable<CountiesRoad>> ListAsync()
        {
            return await _context.CountiesRoads.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<CountiesRoad>> ListAsync(string RoadNumber)
        {
            return await _context.CountiesRoads
                .Where(s => s.RdNum == RoadNumber)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IQueryable<CountiesRoad>> ListViewAsync(long AuthorityId)
        {
            IQueryable<CountiesRoad> x = null;
            await Task.Run(() =>
            {
                x = _context.CountiesRoads
                .Where(w=>w.Road.AuthorityId== AuthorityId)
                .OrderBy(o => o.RdNum);
            }).ConfigureAwait(false);

            return x;
        }

        public void Remove(CountiesRoad countiesRoad)
        {
            _context.CountiesRoads.Remove(countiesRoad);
        }

        public void Update(long ID, CountiesRoad countiesRoad)
        {
            _context.Entry(countiesRoad).State = EntityState.Modified;
        }
    }
}
