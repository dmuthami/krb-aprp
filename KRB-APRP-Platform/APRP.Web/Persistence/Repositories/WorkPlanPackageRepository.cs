using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class WorkPlanPackageRepository : BaseRepository, IWorkPlanPackageRepository
    {
        public WorkPlanPackageRepository(AppDbContext context) : base(context) { }
        public async Task AddAsync(WorkPlanPackage workPlanPackage)
        {
            await _context.WorkPlanPackages.AddAsync(workPlanPackage);
        }

        public async Task<WorkPlanPackage> FindByIdAsync(long ID)
        {
            return await _context.WorkPlanPackages
                .Where(wp => wp.ID == ID)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rd => rd.RoadWorkSectionPlan)
                    .ThenInclude(ro => ro.Road)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rs => rs.RoadWorkSectionPlan)
                    .ThenInclude(s => s.RoadSection)
                    .ThenInclude(w => w.SurfaceType)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                .ThenInclude(rs => rs.RoadWorkSectionPlan)
                    .ThenInclude(s => s.RoadSection)
                    .ThenInclude(s => s.Constituency)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.PlanActivities)
                        .ThenInclude(i=>i.ItemActivityUnitCost)
                        .ThenInclude(g=>g.ItemActivityGroup)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.PlanActivities)
                        .ThenInclude(t => t.Technology)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.PlanActivities)
                        .ThenInclude(g=>g.PackageProgressEntries)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                       .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.WorkCategory)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                       .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.ExecutionMethod)
                .Include(a=>a.Authority)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<WorkPlanPackage> FindByCodeAsync(string code)
        {
            return await _context.WorkPlanPackages
                .Where(wp => wp.Code == code)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rd => rd.RoadWorkSectionPlan)
                    .ThenInclude(ro => ro.Road)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rs => rs.RoadWorkSectionPlan)
                    .ThenInclude(s => s.RoadSection)
                    .ThenInclude(w => w.SurfaceType)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                .ThenInclude(rs => rs.RoadWorkSectionPlan)
                    .ThenInclude(s => s.RoadSection)
                    .ThenInclude(s => s.Constituency)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.PlanActivities)
                        .ThenInclude(i => i.ItemActivityUnitCost)
                        .ThenInclude(g => g.ItemActivityGroup)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.PlanActivities)
                        .ThenInclude(t => t.Technology)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                    .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.PlanActivities)
                        .ThenInclude(g => g.PackageProgressEntries)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                       .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.WorkCategory)
                .Include(r => r.WorkpackageRoadWorkSectionPlans)
                       .ThenInclude(rs => rs.RoadWorkSectionPlan)
                        .ThenInclude(a => a.ExecutionMethod)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }


        public async Task<IEnumerable<WorkPlanPackage>> ListAsync()
        {
            return await _context.WorkPlanPackages.Include(w=>w.WorkpackageRoadWorkSectionPlans).ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkPlanPackage>> ListByFinancialYearAsync(long financialYearId, long authorityID)
        {
            return await _context.WorkPlanPackages
                .Include(s=>s.WorkpackageRoadWorkSectionPlans).ThenInclude(p=>p.RoadWorkSectionPlan).ThenInclude(w=>w.WorkCategory)
                .Where(f => f.FinancialYearId == financialYearId && f.AuthorityId == authorityID).ToListAsync().ConfigureAwait(false);
        }

        public void Remove(WorkPlanPackage workPlanPackage)
        {
            _context.WorkPlanPackages.Remove(workPlanPackage);
        }

        public void Update(WorkPlanPackage workPlanPackage)
        {
            _context.WorkPlanPackages.Update(workPlanPackage);
        }
    }
}
