using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IFundTypeService
    {
        Task<IEnumerable<FundType>> ListAsync();

        Task<FundTypeResponse> AddAsync(FundType fundType);
        Task<FundTypeResponse> FindByIdAsync(long ID);
        Task<FundTypeResponse> Update(FundType fundType);
        Task<FundTypeResponse> RemoveAsync(long ID);
        Task<FundTypeResponse> Update(long ID,FundType fundType);
    }
}
