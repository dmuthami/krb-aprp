using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IDisbursementService
    {
        Task<DisbursementListResponse> ListAsync();
        Task<DisbursementListResponse> ListAsync(long FinancialYearId);
        Task<DisbursementListResponse> ListAsync(long AuthorityId,long FinancialYearId);
        Task<DisbursementListResponse> ListDisbursementByReleaseAsync(long ReleaseId);
        Task<GenericResponse> DisbursementSummaryAsync(long FinancialYearId);
        Task<GenericResponse> DisbursementSummaryByBudgetCeilingComputationAsync(long FinancialYearId);
        Task<DisbursementResponse> AddAsync(Disbursement disbursement);
        Task<DisbursementResponse> FindByIdAsync(long ID);
        Task<DisbursementResponse> FindByDisbursementEntryAsync(Disbursement disbursement);
        Task<DisbursementResponse> FindByAuthorityIdAndFinancialYearIdAsync(long AuthorityId, long FinancialYearId);
        Task<DisbursementResponse> Update(Disbursement disbursement);
        Task<DisbursementResponse> Update(long ID, Disbursement disbursement);
        Task<DisbursementResponse> RemoveAsync(long ID);
        Task<double> DisbursementItemSum(IList<Disbursement> disbursementCollectionList);
        Task<DisbursementResponse> DetachFirstEntryAsync(Disbursement disbursement);
        Task<GenericResponse> SummarizeByFinancialYearIDAndAuthorityIDAsync(long FinancialYearId);
        Task<GenericResponse> SummarizeByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync(long FinancialYearId);
    }
}
