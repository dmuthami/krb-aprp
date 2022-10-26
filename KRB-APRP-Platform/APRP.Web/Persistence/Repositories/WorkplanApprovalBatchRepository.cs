using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class WorkplanApprovalBatchRepository : BaseRepository, IWorkplanApprovalBatchRepository
    {

        public WorkplanApprovalBatchRepository(AppDbContext appDbContext) : base(appDbContext) { }


        public async Task AddAsync(WorkplanApprovalBatch workplanApprovalBatch)
        {
            await _context.WorkplanApprovalBatches.AddAsync(workplanApprovalBatch).ConfigureAwait(false);
        }

        public async Task<WorkplanApprovalBatch> FindByIdAsync(long ID)
        {
            return await _context.WorkplanApprovalBatches.Where(a => a.ID == ID).Include(a=>a.Authority).Include(r=>r.WorkplanApprovalHistories).SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<WorkplanApprovalBatch> FindByFinancialYearAsync(long financialYearId, long authorityId)
        {
            return await _context.WorkplanApprovalBatches
                .Where(a => a.FinancialYearId == financialYearId && a.AuthorityId  == authorityId)
                .Include(r => r.WorkplanApprovalHistories).Include(a=>a.Authority).Include(f=>f.Authority)
                .OrderByDescending(i=>i.ID)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkplanApprovalBatch>> ListAsync()
        {
            return await _context.WorkplanApprovalBatches.Include(h => h.WorkplanApprovalHistories).ToListAsync().ConfigureAwait(false);
        }

        public void Remove(WorkplanApprovalBatch workplanApprovalBatch)
        {
            _context.WorkplanApprovalBatches.Remove(workplanApprovalBatch);
        }

        public void Update(WorkplanApprovalBatch workplanApprovalBatch)
        {
            _context.WorkplanApprovalBatches.Update(workplanApprovalBatch);
        }
    }
}
