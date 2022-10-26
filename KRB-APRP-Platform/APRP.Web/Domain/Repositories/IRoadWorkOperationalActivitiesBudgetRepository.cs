using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadWorkOperationalActivitiesBudgetRepository
    {
        Task<IEnumerable<RoadWorkOperationalActivitiesBudget>> ListAsync(long headerId);
        Task AddAsync(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget);
        Task<RoadWorkOperationalActivitiesBudget> FindByIdAsync(long ID);
        void Update(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget);
        void Remove(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget);
    }
}
