using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IBudgetCeilingComputationService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> ListAsync(long FinancialYearId);
        Task<GenericResponse> ListAsync(string Code);
        Task<GenericResponse> AddAsync(BudgetCeilingComputation budgetCeilingComputation);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> FindByCodeAndFinancialYearIdAsync(string Code, long FinancialYearID);
        Task<GenericResponse> Update(BudgetCeilingComputation budgetCeilingComputation);
        Task<GenericResponse> Update(long ID, BudgetCeilingComputation budgetCeilingComputation);
        Task<GenericResponse> RemoveAsync(long ID);
        Task<GenericResponse> FindByBudgetVoteEntryAsync(BudgetCeilingComputation budgetCeilingComputation);
        Task<GenericResponse> DetachFirstEntryAsync(BudgetCeilingComputation budgetCeilingComputation);
    }
}
