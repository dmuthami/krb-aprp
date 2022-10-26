using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IFinancialProgressService
    {
        Task<IEnumerable<FinancialProgress>> ListByAuthorityIdAsync(long authorityId);
        Task<IEnumerable<FinancialProgress>> ListByAuthorityIdAndFinancialYearAsync(long authorityId, long financialYearId);

        Task<FinancialProgressResponse> AddAsync(FinancialProgress financialProgress);
        Task<FinancialProgressResponse> FindByIdAsync(long ID);
        Task<FinancialProgressResponse> UpdateAsync(FinancialProgress financialProgress);
        Task<FinancialProgressResponse> RemoveAsync(long ID);
    }
}
