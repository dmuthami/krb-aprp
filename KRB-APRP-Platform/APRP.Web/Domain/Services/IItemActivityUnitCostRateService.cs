using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IItemActivityUnitCostRateService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(ItemActivityUnitCostRate itemActivityUnitCostRate);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> Update(ItemActivityUnitCostRate itemActivityUnitCostRate);
        Task<GenericResponse> RemoveAsync(long ID);

        Task<GenericResponse> FindByFinancialYearAuthorityAndItemUnitCostAsync(long FinancialYearId,
            long AuthorityId, long ItemActivityUnitCostId);
    }
}
