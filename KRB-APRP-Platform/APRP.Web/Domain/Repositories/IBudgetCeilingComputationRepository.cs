using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IBudgetCeilingComputationRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> ListAsync(long FinancialYearId);
        Task<IActionResult> ListAsync(string Code);
        Task<IActionResult> AddAsync(BudgetCeilingComputation budgetCeilingComputation);
        Task<IActionResult> FindByIdAsync(long ID);
        Task<IActionResult> FindByCodeAndFinancialYearIdAsync(string Code, long FinancialYearID);
        Task<IActionResult> Update(BudgetCeilingComputation budgetCeilingComputation);
        Task<IActionResult> Update(long ID,BudgetCeilingComputation budgetCeilingComputation);
        Task<IActionResult> Remove(BudgetCeilingComputation budgetCeilingComputation);
        Task<IActionResult> FindByBudgetVoteEntryAsync(BudgetCeilingComputation budgetCeilingComputation);
        Task<IActionResult> DetachFirstEntryAsync(BudgetCeilingComputation budgetCeilingComputation);
    }
}
