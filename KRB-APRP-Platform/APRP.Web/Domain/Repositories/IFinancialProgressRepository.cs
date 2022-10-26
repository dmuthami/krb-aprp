using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IFinancialProgressRepository
    {
        Task<IEnumerable<FinancialProgress>> ListByAuthorityIdAndFinancialYearAsync(long authorityId, long financialYearId);
        Task<IEnumerable<FinancialProgress>> ListByAuthorityIdAsync(long authorityId);
        Task AddAsync(FinancialProgress financialProgress);
        Task<FinancialProgress> FindByIdAsync(long ID);
        void Update(FinancialProgress financialProgress); 
        void Remove(FinancialProgress financialProgress);
    }
}
