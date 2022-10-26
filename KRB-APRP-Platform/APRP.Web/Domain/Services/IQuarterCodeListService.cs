using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IQuarterCodeListService
    {
        Task<QuarterCodeListListResponse> ListAsync();

        Task<QuarterCodeListResponse> AddAsync(QuarterCodeList quarterCodeList);
        Task<QuarterCodeListResponse> FindByIdAsync(long ID);
        Task<QuarterCodeListResponse> FindByNameAsync(string Item);
        Task<QuarterCodeListResponse> Update(QuarterCodeList quarterCodeList);
        Task<QuarterCodeListResponse> Update(long ID, QuarterCodeList quarterCodeList);
        Task<QuarterCodeListResponse> RemoveAsync(long ID);

    }
}
