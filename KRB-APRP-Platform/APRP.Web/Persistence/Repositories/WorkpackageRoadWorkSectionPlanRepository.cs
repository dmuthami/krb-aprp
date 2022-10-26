using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class WorkpackageRoadWorkSectionPlanRepository : BaseRepository, IWorkpackageRoadWorkSectionPlanRepository
    {
        public WorkpackageRoadWorkSectionPlanRepository(AppDbContext context) : base(context) { }
        public async Task AddAsync(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan)
        {
            await _context.WorkpackageRoadWorkSectionPlans.AddAsync(workpackageRoadWorkSectionPlan);
        }

        public async Task<WorkpackageRoadWorkSectionPlan> FindByIdAsync(long ID)
        {
            return await _context.WorkpackageRoadWorkSectionPlans.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<WorkpackageRoadWorkSectionPlan> FindBySectionPlanIdAndWorkPackageId(long sectionPlanId, long workpackageId)
        {
            return await _context.WorkpackageRoadWorkSectionPlans.Where(s=>s.RoadWorkSectionPlanId == sectionPlanId && s.WorkPlanPackageId ==workpackageId).SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public void Remove(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan)
        {
            _context.WorkpackageRoadWorkSectionPlans.Remove(workpackageRoadWorkSectionPlan);
        }

        public void Update(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan)
        {
            _context.WorkpackageRoadWorkSectionPlans.Update(workpackageRoadWorkSectionPlan);
        }
    }
}
