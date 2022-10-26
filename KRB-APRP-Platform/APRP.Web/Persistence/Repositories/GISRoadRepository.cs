using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class GISRoadRepository : BaseRepository , IGISRoadRepository
    {
        public GISRoadRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(GISRoad gISRoad)
        {
           await _context.GISRoads.AddAsync(gISRoad);
        }

        public async Task<GISRoad> FindByRoadNumberAsync(string RoadNumber)
        {
            var gISRoad = await _context.GISRoads
                .Where(s => s.RoadNumber == RoadNumber)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            return gISRoad;            
        }

        public async Task<GISRoad> FindByIdAsync(long ID)
        {
            return await _context.GISRoads.FindAsync(ID);
        }

        public async Task<IEnumerable<GISRoad>> ListAsync()
        {
            return await _context.GISRoads
                //.Include(rs=>rs.RoadSections)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<GISRoad>> ListByRoadNumberAsync(string RoadNumber)
        {
            var gISRoads = await _context.GISRoads
                .Where(s => s.RoadNumber == RoadNumber).
               ToListAsync().ConfigureAwait(false);
            return gISRoads;
        }

        public void Remove(GISRoad gISRoad)
        {
            _context.GISRoads.Remove(gISRoad);
        }

        public void Update(GISRoad gISRoad)
        {
            _context.GISRoads.Update(gISRoad);
        }
    }
}
