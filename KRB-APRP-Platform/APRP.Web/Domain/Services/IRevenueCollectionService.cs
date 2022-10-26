using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRevenueCollectionService
    {
        Task<RevenueCollectionListResponse> ListAsync();
        Task<RevenueCollectionListResponse> ListAsync(long FinancialYearId, string Type);
        Task<RevenueCollectionResponse> AddAsync(RevenueCollection revenueCollection);
        Task<RevenueCollectionResponse> FindByIdAsync(long ID);
        Task<RevenueCollectionResponse> FindByRevenueCollectionCodeUnitIdAsync(long RevenueCollectionCodeUnitId);
        Task<RevenueCollectionResponse> FindByRevenueStreamAndFinancialYearAsync(long FinancialYearID, RevenueStream RevenueStream);
        Task<RevenueCollectionResponse> Update(RevenueCollection revenueCollection);
        Task<RevenueCollectionResponse> Update(long ID, RevenueCollection revenueCollection);
        Task<RevenueCollectionResponse> RemoveAsync(long ID);
        Task<double> RevenueCollectionSum(IList<RevenueCollection> revenueCollectionList);

    }
}
