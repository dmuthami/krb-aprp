using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IContractorService
    {
        Task<IEnumerable<Contractor>> ListAsync();
        Task<ContractorResponse> AddAsync(Contractor contractor);
        Task<ContractorResponse> FindByIdAsync(long ID);
        Task<ContractorResponse> FindByKraPinAsync(string kraPin);
        Task<ContractorResponse> UpdateAsync(Contractor contractor);
        Task<ContractorResponse> RemoveAsync(long ID);
    }
}
