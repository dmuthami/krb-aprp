using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IFinancialYearRepository 
    {
        Task<IEnumerable<FinancialYear>> ListAsync();
        Task<IActionResult> ListAsync(string Code);
        Task AddAsync(FinancialYear financialYear);
        Task<FinancialYear> FindPreviousYearFromCurrentYear(string financialYearCode);
        Task<FinancialYear> FindByIdAsync(long ID);
        Task<FinancialYear> FindCurrentYear();
        void Update(FinancialYear financialYear);
        Task Update(long ID,FinancialYear financialYear);
        Task SetAllToNotCurrent(FinancialYear financialYear);

        void Remove(FinancialYear financialYear);
    }
}
