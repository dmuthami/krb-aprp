using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IBudgetCeilingRepository
    {
        Task<IEnumerable<BudgetCeiling>> ListAsync();
        Task<IEnumerable<BudgetCeiling>> ListAsync(string AuthorityType, long FinancialYearID);
        Task<IEnumerable<BudgetCeiling>> GetCGsByBudgetCeilingHeaderAsync(long BudgetCeilingHeaderID);
        Task<IEnumerable<BudgetCeiling>> RemoveAllAsync(long? BudgetCeilingHeaderID);
        Task AddAsync(BudgetCeiling budgetCeiling);
        Task<BudgetCeiling> FindByIdAsync(long ID);
        Task<BudgetCeiling> FindApprovedByAuthorityForCurrentYear(long financialYear, long authorityID);
        void Update(BudgetCeiling budgetCeiling);
        void Remove(BudgetCeiling budgetCeiling);

        Task<BudgetCeiling> FindByBudgetHeaderIDAndAuthority(long budgetHeaderID, long authorityID);

        void Update(long ID, BudgetCeiling budgetCeiling);

        Task<BudgetCeiling> FindByAuthorityIDAndFinancialYearID(long AuthorityID, long FinancialYearID);
    }
}
