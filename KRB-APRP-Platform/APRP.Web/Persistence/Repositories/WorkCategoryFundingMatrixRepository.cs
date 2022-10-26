using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class WorkCategoryFundingMatrixRepository : BaseRepository, IWorkCategoryFundingMatrixRepository
    {
        public WorkCategoryFundingMatrixRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(WorkCategoryFundingMatrix workCategoryFundingMatrix)
        {
            await _context.WorkCategoryFundingMatrixs.AddAsync(workCategoryFundingMatrix);
        }

        public async Task<WorkCategoryFundingMatrix> FindByAuthorityAndFinancialIdAsync(long AuthorityId,long FinancialYearId)
        {
            return await _context.WorkCategoryFundingMatrixs
            .FirstOrDefaultAsync(m => m.AuthorityId == AuthorityId && m.FinancialYearId==FinancialYearId).ConfigureAwait(false);
        }

        public async Task<WorkCategoryFundingMatrix> FindByIdAsync(long ID)
        {
            return await _context.WorkCategoryFundingMatrixs
                .Include(f => f.FinancialYear)
                .Include(a => a.Authority)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkCategoryFundingMatrix>> ListAsync()
        {
           return await _context.WorkCategoryFundingMatrixs
                 .Include(f=>f.FinancialYear)
                 .Include(a => a.Authority)
                 .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkCategoryFundingMatrix>> ListAsync(long FinancialYearId)
        {
            return await _context.WorkCategoryFundingMatrixs
                    .Where(w=>w.FinancialYearId== FinancialYearId)
                    .Include(f => f.FinancialYear)
                    .Include(a => a.Authority)
                    .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(WorkCategoryFundingMatrix workCategoryFundingMatrix)
        {
            _context.WorkCategoryFundingMatrixs.Remove(workCategoryFundingMatrix);
        }

        public void Update(WorkCategoryFundingMatrix workCategoryFundingMatrix)
        {
            _context.WorkCategoryFundingMatrixs.Update(workCategoryFundingMatrix);
        }

        public void Update(long ID, WorkCategoryFundingMatrix workCategoryFundingMatrix)
        {
            _context.Entry(workCategoryFundingMatrix).State = EntityState.Modified;
        }
    }
}
