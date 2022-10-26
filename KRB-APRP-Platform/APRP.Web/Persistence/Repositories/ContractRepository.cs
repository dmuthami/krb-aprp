using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ContractRepository : BaseRepository, IContractRepository
    {

        public ContractRepository(AppDbContext appDbContext) : base(appDbContext) { }


        public async Task AddAsync(Contract contract)
        {
            await _context.Contracts.AddAsync(contract);
        }

        public async Task<Contract> FindByIdAsync(long ID)
        {
            return await _context.Contracts
                .Where(c=>c.ID == ID)
                .Include(a=>a.WorkPlanPackage).ThenInclude(a=>a.Authority)
                .Include(p => p.WorkPlanPackage).ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                .Include(c => c.Contractor).ThenInclude(c => c.Directors)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(p=>p.RoadWorkSectionPlan)
                            .ThenInclude(a=>a.PlanActivities)
                                .ThenInclude(i=>i.ItemActivityUnitCost)
                                    .ThenInclude(b=>b.ItemActivityGroup)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(p => p.RoadWorkSectionPlan).ThenInclude(a => a.RoadSection).ThenInclude(c=>c.Constituency)
                .Include(p => p.WorkPlanPackage)
                        .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                            .ThenInclude(p => p.RoadWorkSectionPlan).ThenInclude(r => r.Road)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(p => p.RoadWorkSectionPlan).ThenInclude(a => a.PlanActivities).ThenInclude(q=>q.PackageProgressEntries)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(p => p.RoadWorkSectionPlan).ThenInclude(a => a.PlanActivities).ThenInclude(t=>t.Technology)
                .Include(p=>p.WorkPlanPackage).ThenInclude(f=>f.FinancialYear)
                .Include(p => p.WorkPlanPackage).ThenInclude(f => f.WorkpackageRoadWorkSectionPlans).ThenInclude(p=>p.RoadWorkSectionPlan).ThenInclude(e=>e.ExecutionMethod)
                .Include(c=>c.PaymentCertificates)
                .Include(c => c.EmploymentDetails)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<Contract> FindContractByPackageIdAsync(long workpackageId)
        {
            return await _context.Contracts
               .Where(c => c.WorkPlanPackageId == workpackageId)
               .Include(p => p.WorkPlanPackage)
               .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
               .Include(c => c.Contractor)
               .ThenInclude(c => c.Directors)
               .Include(p => p.WorkPlanPackage)
               .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
               .ThenInclude(p => p.RoadWorkSectionPlan).ThenInclude(a => a.PlanActivities).ThenInclude(i => i.ItemActivityUnitCost)
               .Include(p => p.WorkPlanPackage)
               .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
               .ThenInclude(p => p.RoadWorkSectionPlan).ThenInclude(a => a.PlanActivities).ThenInclude(t => t.Technology)
               .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Contract>> ListAsync()
        {
            return await _context.Contracts
                .Include(p=>p.WorkPlanPackage)
                    .ThenInclude(i=>i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r=>r.RoadWorkSectionPlan)
                            .ThenInclude(r=>r.Road)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(r=>r.RoadSection).ThenInclude(s=>s.SurfaceType)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(w=>w.WorkCategory)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(f => f.FundingSource).ThenInclude(c => c.RevenueCollectionCodeUnits)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(w => w.ExecutionMethod)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(c => c.Constituency)
                .Include(c=>c.Contractor)
                    .ThenInclude(c=>c.Directors)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Contract>> ListContractsByAgencyByFinancialYear(long authorityId, long financialYearId)
        {
            return await _context.Contracts.Where(c=>c.WorkPlanPackage.FinancialYearId == financialYearId && c.WorkPlanPackage.AuthorityId == authorityId)
                .Include(a => a.WorkPlanPackage).ThenInclude(a => a.Authority)
                .Include(a => a.WorkPlanPackage).ThenInclude(a => a.FinancialYear)
                .Include(a => a.Contractor)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(r => r.RoadSection)
                                .ThenInclude(s => s.SurfaceType)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(w => w.WorkCategory)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(f => f.FundingSource)
                                .ThenInclude(c => c.RevenueCollectionCodeUnits)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(f => f.FundType)
                .Include(p => p.PaymentCertificates)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(f => f.RoadSection).ThenInclude(c => c.Constituency).ThenInclude(c => c.County)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(f => f.Road)
                .Include(p => p.WorkPlanPackage)
                    .ThenInclude(i => i.WorkpackageRoadWorkSectionPlans)
                        .ThenInclude(r => r.RoadWorkSectionPlan)
                            .ThenInclude(f => f.RoadSection).ThenInclude(s => s.SurfaceType)
                .ToListAsync().ConfigureAwait(false);
        }
        public void Remove(Contract contract)
        {
            _context.Contracts.Remove(contract);
        }

        public void Update(Contract contract)
        {
            _context.Contracts.Update(contract);
        }
    }
}
