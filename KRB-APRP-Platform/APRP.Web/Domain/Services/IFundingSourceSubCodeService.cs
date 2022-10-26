using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IFundingSourceSubCodeService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(FundingSourceSubCode fundingSourceSubCode);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> Update(FundingSourceSubCode fundingSourceSubCode);
        Task<GenericResponse> Update(long ID, FundingSourceSubCode fundingSourceSubCode);
        Task<GenericResponse> RemoveAsync(long ID);
    }
}
