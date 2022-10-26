using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadSheetLengthRepository : BaseRepository, IRoadSheetLengthRepository
    {
        public RoadSheetLengthRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(RoadSheetLength roadSheetLength)
        {
            await _context.RoadSheetLengths.AddAsync(roadSheetLength);
        }

        public async Task<RoadSheetLength> FindByIdAsync(long ID)
        {
            return await _context.RoadSheetLengths
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }


        public async Task<IEnumerable<RoadSheetLength>> ListAsync()
        {
           return await _context.RoadSheetLengths
                 .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadSheetLength roadSheetLength)
        {
            _context.RoadSheetLengths.Remove(roadSheetLength);
        }

        public void Update(RoadSheetLength roadSheetLength)
        {
            _context.RoadSheetLengths.Update(roadSheetLength);
        }

        public void Update(long ID, RoadSheetLength roadSheetLength)
        {
            _context.Entry(roadSheetLength).State = EntityState.Modified;
        }
    }
}
