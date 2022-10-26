using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IReleaseRepository
    {
        Task<IEnumerable<Release>> ListAsync();
        Task<IEnumerable<Release>> ListAsync(long FinancialYearId);
        Task<IEnumerable<Release>> ListAsync(long FinancialYearId,string Code);

        Task<IEnumerable<Release>> ListAsync2(long FinancialYearId, string Code, DateTime startDate,
            DateTime endDate);
        Task AddAsync(Release release);
        Task<Release> FindByIdAsync(long ID);
        Task<Release> FindByReleaseEntryAsync(Release release);

        Task<Disbursement> FindDisbursementByReleaseAsync(Release release);

        Task<IActionResult> ListDisbursementByReleaseAsync(Release release);

        void Update(Release release);
        void Update(long ID, Release release);
        void Remove(Release release);
        Task DetachFirstEntryAsync(Release release);
    }
}
