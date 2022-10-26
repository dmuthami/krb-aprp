using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IReleaseService
    {
        Task<ReleaseListResponse> ListAsync();
        Task<ReleaseListResponse> ListAsync(long FinancialYearId);
        Task<ReleaseListResponse> ListAsync(long FinancialYearId, string Code);
        Task<ReleaseListResponse> ListAsync2(FinancialYear financialYear, string Code);
        Task<ReleaseResponse> AddAsync(Release release);
        Task<ReleaseResponse> FindByIdAsync(long ID);
        Task<ReleaseResponse> FindByReleaseEntryAsync(Release release);
        Task<DisbursementResponse> FindDisbursementByReleaseAsync(Release release);
        Task<GenericResponse> ListDisbursementByReleaseAsync(Release release);
        Task<ReleaseResponse> Update(Release release);
        Task<ReleaseResponse> Update(long ID, Release release);
        Task<ReleaseResponse> RemoveAsync(long ID);
        Task<ReleaseResponse> DetachFirstEntryAsync(Release release);
 
    }
}
