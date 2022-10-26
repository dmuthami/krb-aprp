using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadSheetLengthRepository
    {
        Task<IEnumerable<RoadSheetLength>> ListAsync();

        Task AddAsync(RoadSheetLength roadSheetLength);
        Task<RoadSheetLength> FindByIdAsync(long ID);

        void Update(RoadSheetLength roadSheetLength);
        void Update(long ID, RoadSheetLength roadSheetLength);

        void Remove(RoadSheetLength roadSheetLength);
    }
}
