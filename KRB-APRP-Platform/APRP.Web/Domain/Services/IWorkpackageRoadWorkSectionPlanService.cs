using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IWorkpackageRoadWorkSectionPlanService
    {
        
        Task<WorkpackageRoadWorkSectionPlanResponse> AddAsync(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan);
        Task<WorkpackageRoadWorkSectionPlanResponse> FindByIdAsync(long ID);
        Task<WorkpackageRoadWorkSectionPlanResponse> UpdateAsync(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan);
        Task<WorkpackageRoadWorkSectionPlanResponse> RemoveAsync(long ID);
        Task<WorkpackageRoadWorkSectionPlanResponse> FindBySectionPlanIdAndWorkPackageId(long sectionPlanId, long workpackageId);
    }
}
