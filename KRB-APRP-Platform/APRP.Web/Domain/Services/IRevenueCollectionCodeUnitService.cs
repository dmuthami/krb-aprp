using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRevenueCollectionCodeUnitService
    {
        Task<RevenueCollectionCodeUnitListResponse> ListAsync(long? AuthorityId);
        Task<RevenueCollectionCodeUnitListResponse> ListAsync(long FinancialYearId,string Type);
        Task<RevenueCollectionCodeUnitResponse> AddAsync(RevenueCollectionCodeUnit revenueCollectionCodeUnit);
        Task<RevenueCollectionCodeUnitResponse> FindByIdAsync(long ID);
        Task<RevenueCollectionCodeUnitResponse> FindByNameAsync(RevenueStream RevenueStream);
        Task<RevenueCollectionCodeUnitResponse> FindByNameAsync(RevenueStream RevenueStream, long FinancialYearId);
        Task<RevenueCollectionCodeUnitResponse> FindByNameAsync(RevenueStream RevenueStream, long FinancialYearId, long AuthorityId);
        Task<RevenueCollectionCodeUnitResponse> Update(RevenueCollectionCodeUnit revenueCollectionCodeUnit);
        Task<RevenueCollectionCodeUnitResponse> Update(long ID, RevenueCollectionCodeUnit revenueCollectionCodeUnit);
        Task<RevenueCollectionCodeUnitResponse> RemoveAsync(long ID);

    }
}
