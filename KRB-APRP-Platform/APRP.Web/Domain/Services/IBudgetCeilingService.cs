using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IBudgetCeilingService
    {
        Task<IEnumerable<BudgetCeiling>> ListAsync();
        Task<IEnumerable<BudgetCeiling>> ListAsync(string AuthorityType, long FinancialYearID);
        Task<BudgetCeilingResponse> AddAsync(BudgetCeiling budgetCeiling);
        Task<BudgetCeilingResponse> FindByIdAsync(long ID);
        Task<BudgetCeilingResponse> FindApprovedByAuthorityForCurrentYear(long financialYear, long authorityID);
        Task<BudgetCeilingResponse> Update(FinancialYear financialYear);
        Task<BudgetCeilingResponse> RemoveAsync(long ID);
        Task<BudgetCeilingListResponse> RemoveAllAsync(long? BudgetCeilingHeaderID);
        Task<BudgetCeilingResponse> FindByBudgetHeaderIDAndAuthority(long budgetHeaderID, long authorityID);
        Task<BudgetCeilingResponse> Update(long ID, BudgetCeiling budgetCeiling);
        Task<BudgetCeilingResponse> FindByAuthorityIDAndFinancialYearID(long AuthorityID, long FinancialYearID);
    }
}
