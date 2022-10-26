using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IFundingSourceService
    {
        Task<IEnumerable<FundingSource>> ListAsync();
        Task<FundingSourceResponse> AddAsync(FundingSource fundingSource);
        Task<FundingSourceResponse> FindByIdAsync(long ID);
        Task<FundingSourceResponse> Update(FundingSource fundingSource);
        Task<FundingSourceResponse> Update(long ID,FundingSource fundingSource);
        Task<FundingSourceResponse> RemoveAsync(long ID);
        Task<FundingSourceResponse> FindByNameAsync(string Name);
    }
}
