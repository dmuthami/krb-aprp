using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadWorkOperationalActivitiesBudgetRepository : BaseRepository, IRoadWorkOperationalActivitiesBudgetRepository
    {

        public RoadWorkOperationalActivitiesBudgetRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget)
        {
            await _context.RoadWorkOperationalActivitiesBudgets.AddAsync(roadWorkOperationalActivitiesBudget);
        }

        public async Task<RoadWorkOperationalActivitiesBudget> FindByIdAsync(long ID)
        {
            return await _context.RoadWorkOperationalActivitiesBudgets.FindAsync(ID);
        }

        public async Task<IEnumerable<RoadWorkOperationalActivitiesBudget>> ListAsync(long headerId)
        {
            return await _context.RoadWorkOperationalActivitiesBudgets
                .Where(bLine => bLine.RoadWorkBudgetHeaderId == headerId)
                .Include(fundingSource => fundingSource.FundingSource)
                .Include(fundType => fundType.FundType)
                .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget)
        {
            _context.RoadWorkOperationalActivitiesBudgets.Remove(roadWorkOperationalActivitiesBudget);
        }

        public void Update(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget)
        {
            _context.RoadWorkOperationalActivitiesBudgets.Update(roadWorkOperationalActivitiesBudget);
        }
    }
}
