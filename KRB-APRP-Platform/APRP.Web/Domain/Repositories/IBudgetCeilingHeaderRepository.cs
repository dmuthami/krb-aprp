using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IBudgetCeilingHeaderRepository
    {
        Task<IEnumerable<BudgetCeilingHeader>> ListAsync();
        Task AddAsync(BudgetCeilingHeader roadWorkBudgetHeader);
        Task<BudgetCeilingHeader> FindByIdAsync(long ID);
        Task<BudgetCeilingHeader> FindByFinancialYearAsync(long FinancialYearID);
        Task<BudgetCeilingHeader> FindCurrentAsync();
        void Update(BudgetCeilingHeader roadWorkBudgetHeader);

        void Update(long ID, BudgetCeilingHeader budgetCeilingHeader);
        void Remove(BudgetCeilingHeader roadWorkBudgetHeader);
    }
}
