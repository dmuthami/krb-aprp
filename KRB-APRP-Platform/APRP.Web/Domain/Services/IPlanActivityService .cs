using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IPlanActivityService
    {
        Task<IEnumerable<PlanActivity>> ListAsync();

        Task<PlanActivityResponse> AddAsync(PlanActivity planActivity);
        Task<PlanActivityResponse> FindByIdAsync(long ID);
        Task<PlanActivityResponse> UpdateAsync(PlanActivity planActivity);
        Task<PlanActivityResponse> RemoveAsync(long ID);
        Task<PlanActivityResponse> UpdateBulkPackageQuantityByPackageIdAsync(long workpackageId, ApplicationUser user);
        Task<PlanActivityResponse> ResetPlanQuantitiesAsync(long workpackageId, long roadWorkSectionPlanId);
    }
}
