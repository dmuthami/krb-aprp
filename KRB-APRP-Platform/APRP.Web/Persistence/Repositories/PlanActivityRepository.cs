using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class PlanActivityRepository : BaseRepository, IPlanActivityRepository
    {

        public PlanActivityRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(PlanActivity planActivity)
        {
            await _context.PlanActivities.AddAsync(planActivity);
        }

        public async Task<PlanActivity > FindByIdAsync(long ID)
        {
            return await _context.PlanActivities
                .Include(u => u.ItemActivityUnitCost)
                .Include(t=>t.Technology)
                .Include(s => s.RoadWorkSectionPlan).ThenInclude(r=>r.Road)
                .FirstOrDefaultAsync(i => i.ID == ID)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PlanActivity>> ListAsync()
        {
            return await _context.PlanActivities
                .Include(item=> item.ItemActivityUnitCost)
                .Include(section=>section.RoadWorkSectionPlan)
                .Include(T=>T.Technology)
                .OrderByDescending(p=>p.ItemActivityUnitCost.ItemCode).ToListAsync().ConfigureAwait(false);
        }

        public void Remove(PlanActivity planActivity)
        {
            _context.PlanActivities.Remove(planActivity);
        }

        public void Update(PlanActivity planActivity)
        {
            _context.PlanActivities.Update(planActivity);
        }
    }
}
