using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IContractService
    {
        Task<IEnumerable<Contract>> ListAsync();
        Task<IEnumerable<Contract>> ListContractsByAgencyByFinancialYear(long authorityId, long financialYearId);
        Task<ContractResponse> AddAsync(Contract contract);
        Task<ContractResponse> FindByIdAsync(long ID);
        Task<ContractResponse> FindContractByPackageIdAsync(long workpackageId);
        Task<ContractResponse> UpdateAsync(Contract contract);
        Task<ContractResponse> RemoveAsync(long ID);
    }
}
