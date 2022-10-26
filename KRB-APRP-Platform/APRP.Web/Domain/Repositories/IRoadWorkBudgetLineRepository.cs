using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadWorkBudgetLineRepository
    {
        Task<IEnumerable<RoadWorkBudgetLine>> ListAsync(long headerId);
        Task AddAsync(RoadWorkBudgetLine roadWorkBudgetLine);
        Task<RoadWorkBudgetLine> FindByIdAsync(long ID);
        Task<RoadWorkBudgetLine> FindByRoadWorkBudgetHeaderIdAsync(long RoadWorkBudgetHeaderId);
        void Update(RoadWorkBudgetLine roadWorkBudgetLine);
        void Remove(RoadWorkBudgetLine roadWorkBudgetLine);
    }
}
