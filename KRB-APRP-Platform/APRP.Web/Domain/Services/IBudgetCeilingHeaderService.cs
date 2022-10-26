using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IBudgetCeilingHeaderService
    {
        Task<IEnumerable<BudgetCeilingHeader>> ListAsync();

        Task<BudgetCeilingHeaderResponse> AddAsync(BudgetCeilingHeader budgetCeilingHeader);
        Task<BudgetCeilingHeaderResponse> FindByIdAsync(long ID);
        Task<BudgetCeilingHeaderResponse> FindByFinancialYearAsync(long FinancialYearID);
        Task<BudgetCeilingHeaderResponse> FindCurrentAsync();
        Task<BudgetCeilingHeaderResponse> Update(BudgetCeilingHeader budgetCeilingHeader);

        Task<BudgetCeilingHeaderResponse> Update(long ID, BudgetCeilingHeader budgetCeilingHeader);
        Task<BudgetCeilingHeaderResponse> RemoveAsync(long ID); 
    }
}
