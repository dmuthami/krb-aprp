using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRevenueCollectionCodeUnitTypeRepository
    {
        Task<IEnumerable<RevenueCollectionCodeUnitType>> ListAsync();

        Task AddAsync(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);
        Task<RevenueCollectionCodeUnitType> FindByIdAsync(long ID);

        Task<RevenueCollectionCodeUnitType> FindByNameAsync(string Type);

        void Update(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);
        void Update(long ID, RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);

        void Remove(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);

        Task DetachFirstEntryAsync(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);
    }
}
