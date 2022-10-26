using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class AllocationRepository : BaseRepository, IAllocationRepository
    {
        public AllocationRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(Allocation allocation)
        {
            await _context.Allocations.AddAsync(allocation).ConfigureAwait(false);
        }


        public async Task<Allocation> FindByIdAsync(long ID)
        {
            return await _context.Allocations            
            .Include(r => r.AllocationCodeUnit)
            .ThenInclude(a=>a.Authority)
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<Allocation> FindByAllocationCodeUnitIdAsync(long AllocationCodeUnitId)
        {
            return await _context.Allocations
            .Include(r => r.AllocationCodeUnit)          
            .FirstOrDefaultAsync(m => m.AllocationCodeUnitId == AllocationCodeUnitId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Allocation>> ListAsync()
        {
           return await _context.Allocations
                      .Include(r => r.AllocationCodeUnit)
                       .Include(r => r.AllocationCodeUnit)
                        .ThenInclude(r => r.Authority)
                     .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Allocation>> ListAsync(long FinancialYearId)
        {
            return await _context.Allocations
                .Where(w=>w.FinancialYearId== FinancialYearId)
                      .Include(r => r.AllocationCodeUnit)
                       .Include(r => r.AllocationCodeUnit)
                        .ThenInclude(r => r.Authority)
                      .ToListAsync()
                      .ConfigureAwait(false);
        }

        public void Remove(Allocation allocation)
        {
            _context.Allocations.Remove(allocation);
        }

        public void Update(Allocation allocation)
        {
            _context.Allocations.Update(allocation);
        }

        public void Update(long ID, Allocation allocation)
        {
            _context.Entry(allocation).State = EntityState.Modified;
        }
    }
}
