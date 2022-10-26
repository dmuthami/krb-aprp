using APRP.Web.Domain.Models;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Domain.Repositories
{
    public interface IRoadSheetRepository
    {
        Task<IEnumerable<RoadSheet>> ListAsync();
        Task AddAsync(RoadSheet roadSheet);
        Task<RoadSheet> FindByIdAsync(long ID);
        void Update(RoadSheet roadSheet);

        void Update(long ID, RoadSheet roadSheet);
        void Remove(RoadSheet roadSheet);
        Task<IEnumerable<RoadSheet>> ListByRoadSectionIdAsync(long RoadSectionID, int? Year);

        Task<IEnumerable<RoadSheet>> DisplayRoadsheetsAsync(long RoadSectionID, int Year);

        Task<RoadSheet> CheckRoadSheetsForYear(RoadSheetVM _RoadSheetVM);
    }
}
