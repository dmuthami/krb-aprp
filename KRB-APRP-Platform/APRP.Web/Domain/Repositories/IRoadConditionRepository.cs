using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadConditionRepository
    {
        Task<IEnumerable<RoadCondition>> ListAsync();
        Task<IEnumerable<RoadCondition>> ListAsync(int? Year);
        Task<IEnumerable<RoadCondition>> ListAsync(Authority authority,int? Year);
        Task AddAsync(RoadCondition roadCondtion);
        Task<RoadCondition> FindByIdAsync(long ID);
        Task<RoadCondition> FindByRoadIdAsync(long RoadID, int? Year);

        void Update(long ID, RoadCondition roadCondtion);

        void Remove(RoadCondition roadCondtion);

        Task<RoadCondition> GetRoadConditionByYear(Road road, int? Year);

        Task<RoadCondition> FindByPriorityRateAsync(long AuthorityID, int? Year, long PriorityRate);

        Task DetachFirstEntryAsync(RoadCondition roadCondtion);
    }
}
