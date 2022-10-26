using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IQuarterCodeUnitService
    {
        Task<QuarterCodeUnitListResponse> ListAsync();
        Task<QuarterCodeUnitListResponse> ListAsync(long FinancialYearId);
        Task<QuarterCodeUnitResponse> AddAsync(QuarterCodeUnit quarterCodeUnit);
        Task<QuarterCodeUnitResponse> FindByIdAsync(long ID);
        Task<QuarterCodeUnitResponse> Update(QuarterCodeUnit quarterCodeUnit);
        Task<QuarterCodeUnitResponse> Update(long ID, QuarterCodeUnit quarterCodeUnit);
        Task<QuarterCodeUnitResponse> RemoveAsync(long ID);
        Task<QuarterCodeUnitResponse> FindByQuarterCodeListIdAndFinancialIdAsync(long QuarterCodeListId, long FinancialYearId);

    }
}
