using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadClassCodeUnitRepository
    {
        Task<IEnumerable<RoadClassCodeUnit>> ListAsync();

        Task AddAsync(RoadClassCodeUnit roadClassCodeUnit);
        Task<RoadClassCodeUnit> FindByIdAsync(long ID);

        Task<RoadClassCodeUnit> FindByNameAsync(string RoadClass);

        void Update(RoadClassCodeUnit roadClassCodeUnit);
        void Update(long ID, RoadClassCodeUnit roadClassCodeUnit);

        void Remove(RoadClassCodeUnit roadClassCodeUnit);

    }
}
