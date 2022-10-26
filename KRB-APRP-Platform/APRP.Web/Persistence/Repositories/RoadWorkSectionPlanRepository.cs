using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadWorkSectionPlanRepository : BaseRepository, IRoadWorkSectionPlanRepository
    {
        public RoadWorkSectionPlanRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(RoadWorkSectionPlan roadWorkSectionPlan)
        {
            await _context.RoadWorkSectionPlans.AddAsync(roadWorkSectionPlan).ConfigureAwait(false);
        }

        public async Task<RoadWorkSectionPlan> FindByIdAsync(long ID)
        {
            //return await _context.RoadWorkSectionPlans.Include(s=> s.RoadSection).FirstOrDefaultAsync(s=> s.ID == ID);
            return await _context.RoadWorkSectionPlans
                .Include(c=>c.WorkCategory)
                .Include(w=>w.WorkplanApprovalBatch)
                .Include(s => s.RoadSection)
                .Include(p => p.PlanActivities).ThenInclude(i=>i.ItemActivityUnitCost)
                .Include(p => p.PlanActivities).ThenInclude(t=>t.Technology)
                .Include(pb=>pb.PlanActivityPBCs).ThenInclude(i=>i.ItemActivityPBC).ThenInclude(t=>t.Technology)
                .Include(r => r.Road).ThenInclude(a=>a.Authority)
                .Include(f=>f.FundingSource).ThenInclude(c=>c.RevenueCollectionCodeUnits).ThenInclude(c=>c.RevenueCollectionCodeUnitType)
                .FirstOrDefaultAsync(s => s.ID == ID).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadWorkSectionPlan>> ListAsync(long roadId, long FinancialYear)
        {
            return await _context.RoadWorkSectionPlans
                .Where(r => r.Road.ID == roadId && r.FinancialYear.ID == FinancialYear)
                .Include(w=>w.WorkplanApprovalBatch)
                .Include(f => f.FundingSource).ThenInclude(c => c.RevenueCollectionCodeUnits)
                .Include(ft => ft.FundType)
                .Include(fy => fy.FinancialYear)
                .Include(rs => rs.RoadSection).ThenInclude(srf => srf.SurfaceType)
                .Include(rs => rs.RoadSection).ThenInclude(cons => cons.Constituency)
                .Include(wk => wk.WorkCategory)
                .Include(exc => exc.ExecutionMethod)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadWorkSectionPlan>> ListByAgencyAsync(long authorityId, long financialYearId)
        {
            return await _context.RoadWorkSectionPlans
               .Where(ap => ap.Authority.ID == authorityId && ap.FinancialYearId==financialYearId)
               .Include(w => w.WorkplanApprovalBatch)
               .Include(r=>r.Road).ThenInclude(a=>a.Authority)
               .Include(r=>r.Road).ThenInclude(s=>s.RoadSections)
               .Include(s=>s.RoadSection)
                .Include(f => f.FundingSource).ThenInclude(c => c.RevenueCollectionCodeUnits)
                .Include(f => f.FundingSource).ThenInclude(ct => ct.RevenueCollectionCodeUnits)
               .Include(ft => ft.FundType)
               .Include(fy => fy.FinancialYear)
               .Include(rs => rs.RoadSection).ThenInclude(srf => srf.SurfaceType)
               .Include(rs => rs.RoadSection).ThenInclude(cons => cons.Constituency)
               .Include(p=>p.PlanActivities).ThenInclude(t=>t.Technology)
                .Include(p => p.PlanActivities).ThenInclude(i => i.ItemActivityUnitCost)
                .Include(p => p.PlanActivityPBCs).ThenInclude(i => i.ItemActivityPBC).ThenInclude(a=>a.Technology)
               .Include(wk => wk.WorkCategory)
               .Include(a=>a.Authority).ThenInclude(r=>r.Regions)
               .Include(exc => exc.ExecutionMethod)
               .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadWorkSectionPlan>> ListAgenciesAllAsync(long financialYearId)
        {
            return await _context.RoadWorkSectionPlans
               .Where(ap => ap.FinancialYearId == financialYearId)
               .Include(w => w.WorkplanApprovalBatch)
               .Include(r => r.Road).ThenInclude(a=>a.Authority)
               .Include(r => r.Road).ThenInclude(s => s.RoadSections)
               .Include(s => s.RoadSection)
                .Include(f => f.FundingSource).ThenInclude(c => c.RevenueCollectionCodeUnits)
                .Include(f => f.FundingSource).ThenInclude(ct => ct.RevenueCollectionCodeUnits)
               .Include(ft => ft.FundType)
               .Include(fy => fy.FinancialYear)
               .Include(rs => rs.RoadSection).ThenInclude(srf => srf.SurfaceType)
               .Include(rs => rs.RoadSection).ThenInclude(cons => cons.Constituency)
               .Include(p => p.PlanActivities).ThenInclude(t => t.Technology)
                .Include(p => p.PlanActivities).ThenInclude(i => i.ItemActivityUnitCost)
                .Include(p => p.PlanActivityPBCs).ThenInclude(i => i.ItemActivityPBC).ThenInclude(a => a.Technology)
               .Include(wk => wk.WorkCategory)
               .Include(exc => exc.ExecutionMethod)
               .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadWorkSectionPlan roadWorkSectionPlan)
        {
            _context.RoadWorkSectionPlans.Remove(roadWorkSectionPlan);
        }

        public void Update(RoadWorkSectionPlan roadWorkSectionPlan)
        {
            _context.RoadWorkSectionPlans.Update(roadWorkSectionPlan);
        }
    }
}
