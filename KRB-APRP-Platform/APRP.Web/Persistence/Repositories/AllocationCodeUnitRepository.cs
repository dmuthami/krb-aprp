using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class AllocationCodeUnitRepository : BaseRepository, IAllocationCodeUnitRepository
    {
        public AllocationCodeUnitRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task AddAsync(AllocationCodeUnit allocationCodeUnit)
        {
            await _context.AllocationCodeUnits.AddAsync(allocationCodeUnit).ConfigureAwait(false);
        }

        public async Task<AllocationCodeUnit> FindByAuthorityAsync(long AuthorityId)
        {
            return await _context.AllocationCodeUnits
                 .Include(a=>a.Authority)
                 .FirstOrDefaultAsync(m => m.AuthorityId == AuthorityId).ConfigureAwait(false);
        }

        public async Task<AllocationCodeUnit> FindByIdAsync(long ID)
        {
            return await _context.AllocationCodeUnits
                //.Include(f => f.FinancialYear)
                .Include(a => a.Authority)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<AllocationCodeUnit> FindByNameAsync(string Item)
        {
            return await _context.AllocationCodeUnits
                //.Include(f => f.FinancialYear)
                .Include(a => a.Authority)
                .FirstOrDefaultAsync(m => m.Authority.Code.ToLower() == Item.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AllocationCodeUnit>> ListAsync(string AuthorityType)
        {
            if (AuthorityType == null)
            {
                return await _context.AllocationCodeUnits
                      .Include(a => a.Authority)
                      .ToListAsync().ConfigureAwait(false);
            }
            else
            {
                if (AuthorityType == "RA")
                {
                    return await _context.AllocationCodeUnits
                    .Include(a => a.Authority)
                    .Where(x => x.Authority.Type == 1 || x.Authority.Type == 4 || x.Authority.Type == 0)
                    .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    return await _context.AllocationCodeUnits
                    .Include(a => a.Authority)
                    .Where(x => x.Authority.Type == 2)
                    .ToListAsync().ConfigureAwait(false);
                }
            }

        }

        public void Remove(AllocationCodeUnit allocationCodeUnit)
        {
            _context.AllocationCodeUnits.Remove(allocationCodeUnit);
        }

        public void Update(AllocationCodeUnit allocationCodeUnit)
        {
            _context.AllocationCodeUnits.Update(allocationCodeUnit);
        }

        public void Update(long ID, AllocationCodeUnit allocationCodeUnit)
        {
            _context.Entry(allocationCodeUnit).State = EntityState.Modified;
        }
    }
}
