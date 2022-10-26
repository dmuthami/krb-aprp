using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class FundTypeRepository : BaseRepository, IFundTypeRepository
    {

        public FundTypeRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(FundType fundType)
        {
            await _context.FundTypes.AddAsync(fundType);
        }

        public async Task<FundType> FindByIdAsync(long ID)
        {
            return await _context.FundTypes.FindAsync(ID);
        }

        public async Task<IEnumerable<FundType>> ListAsync()
        {
            return await _context.FundTypes.ToListAsync().ConfigureAwait(false);
        }


        public void Remove(FundType fundType)
        {
            _context.FundTypes.Remove(fundType);
        }

        public void Update(FundType fundType)
        {
            _context.FundTypes.Update(fundType);
        }

        public void Update(long ID, FundType fundType)
        {
            _context.Entry(fundType).State = EntityState.Modified;
        }
    }
}
