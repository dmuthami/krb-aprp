using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IWorkPlanPackageRepository
    {
        Task<IEnumerable<WorkPlanPackage>> ListAsync();
        Task AddAsync(WorkPlanPackage workPlanPackage);
        Task<WorkPlanPackage> FindByIdAsync(long ID);
        Task<WorkPlanPackage> FindByCodeAsync(string code);
        void Update(WorkPlanPackage workPlanPackage);
        void Remove(WorkPlanPackage workPlanPackage );
        Task<IEnumerable<WorkPlanPackage>> ListByFinancialYearAsync(long financialYearId, long authorityID);
    }
}
