using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadConditionCodeUnitService
    {
        Task<RoadConditionCodeUnitListResponse> ListAsync();
        Task<RoadConditionCodeUnitResponse> AddAsync(RoadConditionCodeUnit roadConditionCodeUnit);
        Task<RoadConditionCodeUnitResponse> FindByIdAsync(long ID);
        Task<RoadConditionCodeUnitResponse> FindByRoadConditionAsync(string RoadCondition);
        Task<RoadConditionCodeUnitResponse> Update(RoadConditionCodeUnit roadConditionCodeUnit);
        Task<RoadConditionCodeUnitResponse> Update(long ID, RoadConditionCodeUnit roadConditionCodeUnit);
        Task<RoadConditionCodeUnitResponse> RemoveAsync(long ID);
        Task<RoadConditionCodeUnitResponse> FindBySurfaceTypeIdAsync(long SurfaceTypeId);
    }
}
