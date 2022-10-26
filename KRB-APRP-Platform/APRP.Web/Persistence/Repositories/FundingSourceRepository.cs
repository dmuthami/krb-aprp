using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class FundingSourceRepository : BaseRepository, IFundingSourceRepository
    {

        public FundingSourceRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(FundingSource fundingSource)
        {
            await _context.FundingSources.AddAsync(fundingSource);
        }

        public async Task<FundingSource> FindByIdAsync(long ID)
        {
            return await _context.FundingSources.FindAsync(ID);
        }

        public async Task<FundingSource> FindByNameAsync(string Name)
        {
            return await _context.FundingSources
                .Where(s=>s.Name==Name)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<FundingSource>> ListAsync()
        {
            return await _context.FundingSources.Include(s=>s.FundingSourceSubCodes).ToListAsync().ConfigureAwait(false);
        }

        public void Remove(FundingSource fundingSource)
        {
            _context.FundingSources.Remove(fundingSource);
        }

        public void Update(FundingSource fundingSource)
        {
            _context.FundingSources.Update(fundingSource);
        }

        public void Update(long ID, FundingSource fundingSource)
        {
            _context.Entry(fundingSource).State = EntityState.Modified;
        }
    }
}
