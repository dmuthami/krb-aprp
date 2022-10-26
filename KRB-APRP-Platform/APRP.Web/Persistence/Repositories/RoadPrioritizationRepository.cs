using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadPrioritizationRepository : BaseRepository, IRoadPrioritizationRepository
    {
        public RoadPrioritizationRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(RoadPrioritization roadPrioritization)
        {
            await _context.RoadPrioritizations.AddAsync(roadPrioritization);
        }

        public async Task<RoadPrioritization> FindByIdAsync(long ID)
        {
            return await _context.RoadPrioritizations
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<RoadPrioritization> FindByNameAsync(string Code)
        {
            return await _context.RoadPrioritizations
            .FirstOrDefaultAsync(m => m.Code.ToLower() == Code.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadPrioritization>> ListAsync()
        {
           return await _context.RoadPrioritizations
                 .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadPrioritization roadPrioritization)
        {
            _context.RoadPrioritizations.Remove(roadPrioritization);
        }

        public void Update(RoadPrioritization roadPrioritization)
        {
            _context.RoadPrioritizations.Update(roadPrioritization);
        }

        public void Update(long ID, RoadPrioritization roadPrioritization)
        {
            _context.Entry(roadPrioritization).State = EntityState.Modified;
        }
    }
}
