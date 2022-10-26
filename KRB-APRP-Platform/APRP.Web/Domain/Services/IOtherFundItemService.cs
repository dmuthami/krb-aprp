using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IOtherFundItemService
    {
        Task<OtherFundItemListResponse> ListAsync();
        Task<OtherFundItemListResponse> ListAsync(long FinancialYearId);
        Task<OtherFundItemResponse> AddAsync(OtherFundItem otherFundItem);
        Task<OtherFundItemResponse> FindByIdAsync(long ID);
        Task<OtherFundItemResponse> FindByNameAsync(string Description);
        Task<OtherFundItemResponse> Update(OtherFundItem otherFundItem);
        Task<OtherFundItemResponse> Update(long ID, OtherFundItem otherFundItem);
        Task<OtherFundItemResponse> RemoveAsync(long ID);
        Task<OtherFundItemResponse> FindByFinancialIdAsync(long FinancialYearId);
    }
}
