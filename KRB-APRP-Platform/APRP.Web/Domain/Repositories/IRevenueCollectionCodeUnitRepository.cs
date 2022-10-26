using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRevenueCollectionCodeUnitRepository
    {
        Task<IEnumerable<RevenueCollectionCodeUnit>> ListAsync(long? AuthorityId);

        Task<IEnumerable<RevenueCollectionCodeUnit>> ListAsync(long FinancialYearId, string Type);

        Task AddAsync(RevenueCollectionCodeUnit revenueCollectionCodeUnit);
        Task<RevenueCollectionCodeUnit> FindByIdAsync(long ID);

        Task<RevenueCollectionCodeUnit> FindByNameAsync(RevenueStream RevenueStream);

        Task<RevenueCollectionCodeUnit> FindByNameAsync(RevenueStream RevenueStream, long FinancialYearId);

        Task<RevenueCollectionCodeUnit> FindByNameAsync(RevenueStream RevenueStream, long FinancialYearId, long AuthorityId);

        void Update(RevenueCollectionCodeUnit revenueCollectionCodeUnit);
        void Update(long ID, RevenueCollectionCodeUnit revenueCollectionCodeUnit);

        void Remove(RevenueCollectionCodeUnit revenueCollectionCodeUnit);
    }
}
