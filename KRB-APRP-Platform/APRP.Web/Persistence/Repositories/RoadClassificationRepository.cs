using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadClassificationRepository : BaseRepository, IRoadClassificationRepository
    {
        public RoadClassificationRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task AddAsync(RoadClassification roadClassification)
        {
            await _context.RoadClassifications.AddAsync(roadClassification);
        }


        public async Task<RoadClassification> FindByIdAsync(long ID)
        {
            return await _context.RoadClassifications
                .Include(r => r.RoadClassCodeUnit)
                .Include(a => a.Authority)
                .Include(s => s.SurfaceType)
                .Include(rc => rc.RoadConditionCodeUnit)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<RoadClassification> FindByNameAsync(string RoadName)
        {
            return await _context.RoadClassifications
                .Include(f => f.RoadClassCodeUnit)
                .Include(a => a.Authority)
                .Include(s => s.SurfaceType)
                .Include(rc => rc.RoadConditionCodeUnit)
                .FirstOrDefaultAsync(m => m.RoadName.ToLower() == RoadName.ToLower()).ConfigureAwait(false);
        }

        public async Task<RoadClassification> FindByRoadIdAsync(string RoadId)
        {
            return await _context.RoadClassifications
                .Include(f => f.RoadClassCodeUnit)
                .Include(a => a.Authority)
                .Include(s => s.SurfaceType)
                .Include(rc => rc.RoadConditionCodeUnit)
                .FirstOrDefaultAsync(m => m.RoadId.ToLower() == RoadId.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadClassification>> ListAsync()
        {
            return await _context.RoadClassifications
                    .Include(f => f.RoadClassCodeUnit)
                    .Include(a => a.Authority)
                    .Include(s => s.SurfaceType)
                    .Include(rc => rc.RoadConditionCodeUnit)
                    .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadClassification>> ListAsync(long AuthorityId)
        {
            return await _context.RoadClassifications
                    .Where(w => w.AuthorityId == AuthorityId)
                    .Include(f => f.RoadClassCodeUnit)
                    .Include(a => a.Authority)
                    .Include(s => s.SurfaceType)
                    .Include(rc => rc.RoadConditionCodeUnit)
                    .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadClassification roadClassification)
        {
            _context.RoadClassifications.Remove(roadClassification);
        }

        public void Update(RoadClassification roadClassification)
        {
            _context.RoadClassifications.Update(roadClassification);
        }

        public void Update(long ID, RoadClassification roadClassification)
        {
            _context.Entry(roadClassification).State = EntityState.Modified;
        }
    }
}
