using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class WorkCategoryAllocationMatrixRepository : BaseRepository, IWorkCategoryAllocationMatrixRepository
    {
        public WorkCategoryAllocationMatrixRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task AddAsync(WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            await _context.WorkCategoryAllocationMatrixs.AddAsync(workCategoryAllocationMatrix);
        }

        public async Task<WorkCategoryAllocationMatrix> FindByAuthorityAndFinancialIdAsync(long AuthorityId, long FinancialYearId, long WorkCategoryId)
        {
            return await _context.WorkCategoryAllocationMatrixs
                .Include(f => f.FinancialYear)
                .Include(a => a.Authority)
                .Include(w => w.WorkCategory)
            .FirstOrDefaultAsync(m => m.AuthorityId == AuthorityId && m.FinancialYearId == FinancialYearId && m.WorkCategoryId == WorkCategoryId).ConfigureAwait(false);
        }

        public async Task<WorkCategoryAllocationMatrix> FindByIdAsync(long ID)
        {
            return await _context.WorkCategoryAllocationMatrixs
                .Include(f => f.FinancialYear)
                .Include(a => a.Authority)
                .Include(w => w.WorkCategory)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<WorkCategoryAllocationMatrix> FindByNameAsync(string Name)
        {
            return await _context.WorkCategoryAllocationMatrixs
                .Include(f => f.FinancialYear)
                .Include(a => a.Authority)
                .Include(w => w.WorkCategory)
                .FirstOrDefaultAsync(m => m.WorkCategory.Name.ToLower() == Name.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkCategoryAllocationMatrix>> GetAuthorityWorkCategoriesAsync(long AuthorityId, long FinancialYearId)
        {
            return await _context.WorkCategoryAllocationMatrixs
                  .Where(s=>s.FinancialYear.ID== FinancialYearId && s.AuthorityId == AuthorityId)
                  .Include(f => f.FinancialYear)
                  .Include(a => a.Authority)
                  .Include(w => w.WorkCategory)
                  .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkCategoryAllocationMatrix>> ListAsync()
        {
            return await _context.WorkCategoryAllocationMatrixs
                  .Include(f => f.FinancialYear)
                  .Include(a => a.Authority)
                  .Include(w => w.WorkCategory)
                  .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkCategoryAllocationMatrix>> ListAsync(long FinancialYearId)
        {
            return await _context.WorkCategoryAllocationMatrixs
                    .Where(w => w.FinancialYearId == FinancialYearId)
                    .Include(f => f.FinancialYear)
                    .Include(a => a.Authority)
                    .Include(w => w.WorkCategory)
                    .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            _context.WorkCategoryAllocationMatrixs.Remove(workCategoryAllocationMatrix);
        }

        public void Update(WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            _context.WorkCategoryAllocationMatrixs.Update(workCategoryAllocationMatrix);
        }

        public void Update(long ID, WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            _context.Entry(workCategoryAllocationMatrix).State = EntityState.Modified;
        }
    }
}
