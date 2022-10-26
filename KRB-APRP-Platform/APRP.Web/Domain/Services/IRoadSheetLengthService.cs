using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadSheetLengthService
    {
        Task<RoadSheetLengthListResponse> ListAsync();
        Task<RoadSheetLengthResponse> AddAsync(RoadSheetLength roadSheetLength);
        Task<RoadSheetLengthResponse> FindByIdAsync(long ID);
        Task<RoadSheetLengthResponse> Update(RoadSheetLength roadSheetLength);
        Task<RoadSheetLengthResponse> Update(long ID, RoadSheetLength roadSheetLength);
        Task<RoadSheetLengthResponse> RemoveAsync(long ID);
    }
}
