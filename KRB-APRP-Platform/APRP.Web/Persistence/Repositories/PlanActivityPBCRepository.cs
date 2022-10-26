using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class PlanActivityPBCRepository : BaseRepository, IPlanActivityPBCRepository
    {

        public PlanActivityPBCRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(PlanActivityPBC planActivityPBC )
        {
            await _context.PlanActivityPBCs.AddAsync(planActivityPBC);
        }

        public async Task<PlanActivityPBC> FindByIdAsync(long ID)
        {
            return await _context.PlanActivityPBCs
                .Include(a=>a.ItemActivityPBC)
                .Include(s => s.RoadWorkSectionPlan).ThenInclude(r=>r.Road)
                .FirstOrDefaultAsync(i => i.ID == ID)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PlanActivityPBC>> ListAsync()
        {
            return await _context.PlanActivityPBCs
                .Include(section=>section.RoadWorkSectionPlan).ToListAsync().ConfigureAwait(false);
        }

        public void Remove(PlanActivityPBC planActivityPBC)
        {
            _context.PlanActivityPBCs.Remove(planActivityPBC);
        }

        public void Update(PlanActivityPBC planActivityPBC)
        {
            _context.PlanActivityPBCs.Update(planActivityPBC);
        }
    }
}
