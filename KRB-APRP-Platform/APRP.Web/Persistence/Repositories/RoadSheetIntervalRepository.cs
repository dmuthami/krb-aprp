using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadSheetIntervalRepository : BaseRepository, IRoadSheetIntervalRepository
    {
        public RoadSheetIntervalRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(RoadSheetInterval roadSheetInterval)
        {
            await _context.RoadSheetIntervals.AddAsync(roadSheetInterval);
        }

        public async Task<RoadSheetInterval> FindByIdAsync(long ID)
        {
            return await _context.RoadSheetIntervals
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }


        public async Task<IEnumerable<RoadSheetInterval>> ListAsync()
        {
           return await _context.RoadSheetIntervals
                 .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadSheetInterval roadSheetInterval)
        {
            _context.RoadSheetIntervals.Remove(roadSheetInterval);
        }

        public void Update(RoadSheetInterval roadSheetInterval)
        {
            _context.RoadSheetIntervals.Update(roadSheetInterval);
        }

        public void Update(long ID, RoadSheetInterval roadSheetInterval)
        {
            _context.Entry(roadSheetInterval).State = EntityState.Modified;
        }
    }
}
