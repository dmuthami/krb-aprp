using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadSheetIntervalService
    {
        Task<RoadSheetIntervalListResponse> ListAsync();
        Task<RoadSheetIntervalResponse> AddAsync(RoadSheetInterval roadSheetInterval);
        Task<RoadSheetIntervalResponse> FindByIdAsync(long ID);
        Task<RoadSheetIntervalResponse> Update(RoadSheetInterval roadSheetInterval);
        Task<RoadSheetIntervalResponse> Update(long ID, RoadSheetInterval roadSheetInterval);
        Task<RoadSheetIntervalResponse> RemoveAsync(long ID);
    }
}
