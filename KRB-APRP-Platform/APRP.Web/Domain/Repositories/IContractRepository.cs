using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> ListAsync();
        Task<IEnumerable<Contract>> ListContractsByAgencyByFinancialYear(long authorityId, long financialYearId);
        Task AddAsync(Contract contract);
        Task<Contract> FindByIdAsync(long ID);
        Task<Contract> FindContractByPackageIdAsync(long workpackageId);
        void Update(Contract contract);
        void Remove(Contract contract);
    }
}
