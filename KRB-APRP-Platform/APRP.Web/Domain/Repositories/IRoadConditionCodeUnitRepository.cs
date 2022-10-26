using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadConditionCodeUnitRepository
    {
        Task<IEnumerable<RoadConditionCodeUnit>> ListAsync();

        Task AddAsync(RoadConditionCodeUnit roadClassCodeUnit);
        Task<RoadConditionCodeUnit> FindByIdAsync(long ID);

        Task<RoadConditionCodeUnit> FindByRoadConditionAsync(string RoadCondition);

        void Update(RoadConditionCodeUnit roadClassCodeUnit);
        void Update(long ID, RoadConditionCodeUnit roadClassCodeUnit);

        void Remove(RoadConditionCodeUnit roadClassCodeUnit);

        Task<RoadConditionCodeUnit> FindBySurfaceTypeIdAsync(long SurfaceTypeId);
    }
}
