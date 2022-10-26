using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRevenueCollectionCodeUnitTypeService
    {
        Task<RevenueCollectionCodeUnitTypeListResponse> ListAsync();
        Task<RevenueCollectionCodeUnitTypeResponse> AddAsync(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);
        Task<RevenueCollectionCodeUnitTypeResponse> FindByIdAsync(long ID);
        Task<RevenueCollectionCodeUnitTypeResponse> Update(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);
        Task<RevenueCollectionCodeUnitTypeResponse> Update(long ID, RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);
        Task<RevenueCollectionCodeUnitTypeResponse> RemoveAsync(long ID);
        Task<RevenueCollectionCodeUnitTypeResponse> FindByNameAsync(string Type);
        Task<RevenueCollectionCodeUnitTypeResponse> DetachFirstEntryAsync(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType);
    }
}
