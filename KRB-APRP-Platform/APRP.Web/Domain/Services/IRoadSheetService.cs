using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.UserViewModels;

namespace APRP.Web.Domain.Services
{
    public interface IRoadSheetService
    {
        Task<RoadSheetListResponse> ListAsync();
        Task<RoadSheetResponse> AddAsync(RoadSheet roadSheet);
        Task<RoadSheetResponse> FindByIdAsync(long ID);
        Task<RoadSheetResponse> Update(RoadSheet roadSheet);
        Task<RoadSheetResponse> Update(long ID, RoadSheet roadSheet);
        Task<RoadSheetResponse> RemoveAsync(long ID);
        Task<RoadSheetListResponse> ListByRoadSectionIdAsync(long RoadSectionID,int? Year);
        Task<RoadSheetListResponse> CreateRoadSheets(double roadLengthKM, double sectionLengthKM, long RoadSectionID, int Year);
        Task<RoadSheetListResponse> DisplayRoadsheetsAsync(long RoadSectionID, int Year);
        Task<RoadSheetResponse> CheckRoadSheetsForYear(RoadSheetVM _RoadSheetVM);
    }
}
