using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadConditionService
    {
        Task<RoadConditionListResponse> ListAsync();
        Task<RoadConditionListResponse> ListAsync(int? Year);
        Task<RoadConditionListResponse> ListAsync(Authority authority, int? Year);
        Task<RoadCondtionResponse> AddAsync(RoadCondition roadCondtion);
        Task<RoadCondtionResponse> FindByIdAsync(long ID);
        Task<RoadCondtionResponse> FindByRoadIdAsync(long RoadID,int? Year);
        Task<RoadCondtionResponse> Update(long ID, RoadCondition roadCondtion);
        Task<RoadCondtionResponse> RemoveAsync(long ID);
        Task<RoadCondtionResponse> GetRoadConditionByYear(Road road,int? Year);
        Task<RoadCondtionResponse> FindByPriorityRateAsync(long AuthorityID, int? Year, long PriorityRate);
        Task<RoadCondtionResponse> DetachFirstEntryAsync(RoadCondition roadCondtion);
    }
}
