using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRoadWorkSectionPlanService
    {
        Task<IEnumerable<RoadWorkSectionPlan>> ListAsync(long roadId, long financialYearId);

        Task<IEnumerable<RoadWorkSectionPlan>> ListByAgencyAsync(long authorityId, long financialYearId);

        Task<IEnumerable<RoadWorkSectionPlan>> ListAgenciesAllAsync(long financialYearId);

        Task<RoadWorkSectionPlanResponse> AddAsync(RoadWorkSectionPlan roadWorkSectionPlan);
        Task<RoadWorkSectionPlanResponse> FindByIdAsync(long ID);
        Task<RoadWorkSectionPlanResponse> UpdateAsync(RoadWorkSectionPlan roadWorkSectionPlan); 
        Task UpdateBatchId(long financialYearId, long BatchId, long authorityId, bool isFinal);
        Task<RoadWorkSectionPlanResponse> RemoveAsync(long ID);
    }
}
