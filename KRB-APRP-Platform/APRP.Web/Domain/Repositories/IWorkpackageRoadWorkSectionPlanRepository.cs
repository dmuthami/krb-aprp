using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IWorkpackageRoadWorkSectionPlanRepository
    {
        Task AddAsync(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan);
        Task<WorkpackageRoadWorkSectionPlan> FindByIdAsync(long ID);
        void Update(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan);
        void Remove(WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan);
        Task<WorkpackageRoadWorkSectionPlan> FindBySectionPlanIdAndWorkPackageId(long sectionPlanId, long workpackageId);
    }
}
