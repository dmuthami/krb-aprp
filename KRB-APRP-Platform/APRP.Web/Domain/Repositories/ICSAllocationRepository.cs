using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface ICSAllocationRepository
    {
        Task<IEnumerable<CSAllocation>> ListAsync();

        Task<IEnumerable<CSAllocation>> ListAsync(long FinancialYearId);

        Task<IEnumerable<CSAllocation>> ListAsync(long AuthorityId, long FinancialYearId);

        Task<IActionResult> CSAllocationSummaryAsync(long FinancialYearId);

        Task<IActionResult> CSAllocationSummaryByBudgetCeilingComputationAsync(long FinancialYearId);

        Task AddAsync(CSAllocation cSAllocation);
        Task<CSAllocation> FindByIdAsync(long ID);
        Task<CSAllocation> FindByCSAllocationEntryAsync(CSAllocation cSAllocation);
        void Update(CSAllocation cSAllocation);
        void Update(long ID, CSAllocation cSAllocation);
        void Remove(CSAllocation cSAllocation);
        Task DetachFirstEntryAsync(CSAllocation cSAllocation);
    }
}
