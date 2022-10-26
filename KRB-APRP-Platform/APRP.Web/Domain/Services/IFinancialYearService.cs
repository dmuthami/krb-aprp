using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IFinancialYearService
    {
        Task<IEnumerable<FinancialYear>> ListAsync();
        Task<GenericResponse> ListAsync(string Code);
        Task<FinancialYearResponse> AddAsync(FinancialYear financialYear);
        Task<FinancialYearResponse> FindPreviousYearFromCurrentYear(FinancialYear financialYear);

        Task<FinancialYearResponse> FindByIdAsync(long ID);
        Task<FinancialYearResponse> FindCurrentYear();
        Task<FinancialYearResponse> Update(FinancialYear financialYear);
        Task<FinancialYearResponse> Update(long ID,FinancialYear financialYear);
        Task<FinancialYearResponse> RemoveAsync(long ID);
    }
}
