using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadWorkBudgetLineRepository : BaseRepository , IRoadWorkBudgetLineRepository
    {

        public RoadWorkBudgetLineRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(RoadWorkBudgetLine roadWorkBudgetLine)
        {
            await _context.RoadWorkBudgetLines.AddAsync(roadWorkBudgetLine);
        }

        public async Task<RoadWorkBudgetLine> FindByIdAsync(long ID)
        {
            return await _context.RoadWorkBudgetLines.FindAsync(ID);
        }

        public async Task<RoadWorkBudgetLine> FindByRoadWorkBudgetHeaderIdAsync(long RoadWorkBudgetHeaderId)
        {
            return await _context.RoadWorkBudgetLines

                .FirstOrDefaultAsync(f=>f.RoadWorkBudgetHeaderId== RoadWorkBudgetHeaderId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RoadWorkBudgetLine>> ListAsync(long headerId)
        {
            return await _context.RoadWorkBudgetLines
                .Where(bLine=> bLine.RoadWorkBudgetHeaderId == headerId)
                .Include(fundingSource => fundingSource.FundingSource)
                .Include(fundType => fundType.FundType)
                .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadWorkBudgetLine roadWorkBudgetLine)
        {
            _context.RoadWorkBudgetLines.Remove(roadWorkBudgetLine);
        }

        public void Update(RoadWorkBudgetLine roadWorkBudgetLine)
        {
            _context.RoadWorkBudgetLines.Update(roadWorkBudgetLine);
        }
    }
}
