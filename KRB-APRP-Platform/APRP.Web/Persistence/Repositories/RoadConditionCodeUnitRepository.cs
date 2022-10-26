using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadConditionCodeUnitRepository : BaseRepository, IRoadConditionCodeUnitRepository
    {
        public RoadConditionCodeUnitRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(RoadConditionCodeUnit roadConditionCodeUnit)
        {
            await _context.RoadConditionCodeUnits.AddAsync(roadConditionCodeUnit);
        }

        public async Task<RoadConditionCodeUnit> FindBySurfaceTypeIdAsync(long SurfaceTypeId)
        {
            return await _context.RoadConditionCodeUnits
            .FirstOrDefaultAsync(m => m.SurfaceTypeId == SurfaceTypeId).ConfigureAwait(false);
        }

        public async Task<RoadConditionCodeUnit> FindByIdAsync(long ID)
        {
            return await _context.RoadConditionCodeUnits
                .Include(f => f.SurfaceType)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<RoadConditionCodeUnit> FindByRoadConditionAsync(string RoadCondition)
        {
            return await _context.RoadConditionCodeUnits
                .Include(a => a.SurfaceType)
                .FirstOrDefaultAsync(m => m.RoadCondition.ToLower() == RoadCondition.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadConditionCodeUnit>> ListAsync()
        {
           return await _context.RoadConditionCodeUnits
                 .Include(f=>f.SurfaceType)
                 .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadConditionCodeUnit roadConditionCodeUnit)
        {
            _context.RoadConditionCodeUnits.Remove(roadConditionCodeUnit);
        }

        public void Update(RoadConditionCodeUnit roadConditionCodeUnit)
        {
            _context.RoadConditionCodeUnits.Update(roadConditionCodeUnit);
        }

        public void Update(long ID, RoadConditionCodeUnit roadConditionCodeUnit)
        {
            _context.Entry(roadConditionCodeUnit).State = EntityState.Modified;
        }
    }
}
