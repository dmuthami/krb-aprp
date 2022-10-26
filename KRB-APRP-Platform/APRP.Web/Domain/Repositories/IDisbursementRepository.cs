using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IDisbursementRepository
    {
        Task<IEnumerable<Disbursement>> ListAsync();

        Task<IEnumerable<Disbursement>> ListAsync(long FinancialYearId);

        Task<IEnumerable<Disbursement>> ListAsync(long AuthorityId, long FinancialYearId);

        Task<IEnumerable<Disbursement>> ListDisbursementByReleaseAsync(long ReleaseId);
        Task<IActionResult> DisbursementSummaryAsync(long FinancialYearId);

        Task<IActionResult> DisbursementSummaryByBudgetCeilingComputationAsync(long FinancialYearId);

        Task<IActionResult> SummarizeByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync(long FinancialYearId);
        Task AddAsync(Disbursement disbursement);
        Task<Disbursement> FindByIdAsync(long ID);
        Task<Disbursement> FindByAuthorityIdAndFinancialYearIdAsync(long AuthorityId, long FinancialYearId);
        Task<Disbursement> FindByDisbursementEntryAsync(Disbursement disbursement);
        void Update(Disbursement disbursement);
        void Update(long ID, Disbursement disbursement);
        void Remove(Disbursement disbursement);
        Task DetachFirstEntryAsync(Disbursement disbursement);
    }
}
