using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IWorkPlanPackageService
    {
        Task<IEnumerable<WorkPlanPackage>> ListAsync();
        Task<IEnumerable<WorkPlanPackage>> ListByFinancialYearAsync(long financialYearId, long authorityID);
        Task<WorkPlanPackageResponse> AddAsync(WorkPlanPackage workPlanPackage);
        Task<WorkPlanPackageResponse> FindByIdAsync(long ID);
        Task<WorkPlanPackageResponse> FindByCodeAsync(string code);
        Task<WorkPlanPackageResponse> UpdateAsync(WorkPlanPackage workPlanPackage);
        Task<WorkPlanPackageResponse> RemoveAsync(long ID);
   }
}
