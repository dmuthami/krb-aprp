using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadSheetIntervalRepository
    {
        Task<IEnumerable<RoadSheetInterval>> ListAsync();

        Task AddAsync(RoadSheetInterval roadSheetInterval);
        Task<RoadSheetInterval> FindByIdAsync(long ID);

        void Update(RoadSheetInterval roadSheetInterval);
        void Update(long ID, RoadSheetInterval roadSheetInterval);

        void Remove(RoadSheetInterval roadSheetInterval);
    }
}
