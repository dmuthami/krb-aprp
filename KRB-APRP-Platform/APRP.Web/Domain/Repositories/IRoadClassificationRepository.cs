using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadClassificationRepository
    {
        Task<IEnumerable<RoadClassification>> ListAsync();
        Task<IEnumerable<RoadClassification>> ListAsync(long AuthorityId);
        Task AddAsync(RoadClassification roadClassCodeUnit);
        Task<RoadClassification> FindByIdAsync(long ID);
        Task<RoadClassification> FindByNameAsync(string RoadName);
        Task<RoadClassification> FindByRoadIdAsync(string RoadId);
        void Update(RoadClassification roadClassCodeUnit);
        void Update(long ID, RoadClassification roadClassCodeUnit);
        void Remove(RoadClassification roadClassCodeUnit);
    }
}
