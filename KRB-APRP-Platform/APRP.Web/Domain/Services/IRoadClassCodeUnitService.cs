using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadClassCodeUnitService
    {
        Task<RoadClassCodeUnitListResponse> ListAsync();
        Task<RoadClassCodeUnitResponse> AddAsync(RoadClassCodeUnit roadClassCodeUnit);
        Task<RoadClassCodeUnitResponse> FindByIdAsync(long ID);
        Task<RoadClassCodeUnitResponse> FindByNameAsync(string RoadClass);
        Task<RoadClassCodeUnitResponse> Update(RoadClassCodeUnit roadClassCodeUnit);
        Task<RoadClassCodeUnitResponse> Update(long ID, RoadClassCodeUnit roadClassCodeUnit);
        Task<RoadClassCodeUnitResponse> RemoveAsync(long ID);
    }
}
