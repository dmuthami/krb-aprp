using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadClassCodeUnitRepository : BaseRepository, IRoadClassCodeUnitRepository
    {
        public RoadClassCodeUnitRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(RoadClassCodeUnit roadClassCodeUnit)
        {
            await _context.RoadClassCodeUnits.AddAsync(roadClassCodeUnit);
        }

        public async Task<RoadClassCodeUnit> FindByIdAsync(long ID)
        {
            return await _context.RoadClassCodeUnits
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<RoadClassCodeUnit> FindByNameAsync(string RoadClass)
        {
            return await _context.RoadClassCodeUnits
                .FirstOrDefaultAsync(m => m.RoadClass.ToLower() == RoadClass.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadClassCodeUnit>> ListAsync()
        {
           return await _context.RoadClassCodeUnits
                 .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadClassCodeUnit roadClassCodeUnit)
        {
            _context.RoadClassCodeUnits.Remove(roadClassCodeUnit);
        }

        public void Update(RoadClassCodeUnit roadClassCodeUnit)
        {
            _context.RoadClassCodeUnits.Update(roadClassCodeUnit);
        }

        public void Update(long ID, RoadClassCodeUnit roadClassCodeUnit)
        {
            _context.Entry(roadClassCodeUnit).State = EntityState.Modified;
        }
    }
}
