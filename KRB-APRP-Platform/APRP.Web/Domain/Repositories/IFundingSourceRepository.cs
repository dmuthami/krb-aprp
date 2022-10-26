using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IFundingSourceRepository
    {
        Task<IEnumerable<FundingSource>> ListAsync();
        Task AddAsync(FundingSource fundingSource);
        Task<FundingSource> FindByIdAsync(long ID);
        Task<FundingSource> FindByNameAsync(string Name);
        void Update(FundingSource fundingSource);
        void Update(long ID,FundingSource fundingSource);
        void Remove(FundingSource fundingSource);
    }
}
