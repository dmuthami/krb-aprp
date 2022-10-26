using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRevenueCollectionRepository
    {
        Task<IEnumerable<RevenueCollection>> ListAsync();

        Task<IEnumerable<RevenueCollection>> ListAsync(long FinancialYearId, string Type);
        Task AddAsync(RevenueCollection revenueCollection);
        Task<RevenueCollection> FindByIdAsync(long ID);
        Task<RevenueCollection> FindByRevenueCollectionCodeUnitIdAsync(long RevenueCollectionCodeUnitId);
        Task<RevenueCollection> FindByRevenueStreamAndFinancialYearAsync(long FinancialYearID, RevenueStream RevenueStream);
        void Update(RevenueCollection revenueCollection);
        void Update(long ID, RevenueCollection revenueCollection);

        void Remove(RevenueCollection revenueCollection);
    }
}
