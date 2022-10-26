using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IFundTypeRepository
    {
        Task<IEnumerable<FundType>> ListAsync();
        Task AddAsync(FundType fundType);
        Task<FundType> FindByIdAsync(long ID);
        void Update(FundType fundType);
        void Remove(FundType fundType);
        void Update(long ID,FundType fundType);
    }
}
