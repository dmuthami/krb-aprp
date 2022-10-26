using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IPlanActivityPBCService
    {
        Task<IEnumerable<PlanActivityPBC>> ListAsync();

        Task<PlanActivityPBCResponse> AddAsync(PlanActivityPBC planActivityPBC);
        Task<PlanActivityPBCResponse> FindByIdAsync(long ID);
        Task<PlanActivityPBCResponse> UpdateAsync(PlanActivityPBC planActivityPBC);
        Task<PlanActivityPBCResponse> RemoveAsync(long ID);
    }
}
